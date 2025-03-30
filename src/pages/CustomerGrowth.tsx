import { useState } from 'react';
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import ServiceUsageChart from '@/components/customer/ServiceUsageChart';
import CustomerTable from '@/components/customer/CustomerTable';
import CustomerFormDialog from '@/components/customer/CustomerFormDialog';
import { useCustomers } from '@/application/hooks/useCustomers';
import { useToast } from '@/hooks/use-toast';
import { Customer } from '@/domain/models/Customer';
import { supabase } from '@/integrations/supabase/client';

export default function CustomerGrowth() {
  const { 
    customers, 
    services, 
    serviceUsageData, 
    isLoading, 
    error,
    calculateBrandGrowth,
    calculateMonthlyGrowth,
    calculateMonthlyGrowthByBrand
  } = useCustomers();
  const { toast } = useToast();

  const handleAddCustomer = async (newCustomer: Customer) => {
    try {
      const { data: insertedCustomer, error: customerError } = await supabase
        .from('customers')
        .insert({
          brand_name: newCustomer.brand_name,
          contact_name: newCustomer.contact_name,
          contact_email: newCustomer.contact_email,
          contact_phone: newCustomer.contact_phone,
          customer_type: newCustomer.customer_type || 'client'
        })
        .select('id')
        .single();

      if (customerError || !insertedCustomer) throw customerError;

      if (newCustomer.acquired_services && newCustomer.acquired_services.length > 0) {
        const { data: serviceData, error: servicesError } = await supabase
          .from('services')
          .select('id, name')
          .in('name', newCustomer.acquired_services);
        
        if (servicesError || !serviceData) throw servicesError;
        
        const customerServiceEntries = serviceData.map(service => ({
          customer_id: insertedCustomer.id,
          service_id: service.id
        }));
        
        const { error: relationError } = await supabase
          .from('customer_services')
          .insert(customerServiceEntries);
        
        if (relationError) throw relationError;
      }

      toast({
        title: "Cliente añadido",
        description: "El cliente ha sido añadido exitosamente.",
      });
    } catch (error) {
      console.error('Error adding customer:', error);
      toast({
        title: "Error",
        description: "No se pudo añadir el cliente. Por favor intente nuevamente.",
        variant: "destructive",
      });
    }
  };

  if (error) {
    return (
      <div className="min-h-screen flex flex-col bg-background">
        <Navbar />
        <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
          <div className="w-[900px] max-w-[900px] h-full bg-white p-6">
            <div className="text-center py-10">
              <h2 className="text-xl font-semibold text-destructive">Error cargando datos de clientes</h2>
              <p className="mt-2 text-muted-foreground">Por favor intente nuevamente más tarde o contacte soporte.</p>
            </div>
          </div>
        </main>
        <Footer />
      </div>
    );
  }

  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
        <div className="w-[900px] max-w-[900px] h-full bg-white p-6">
          <div className="flex justify-between items-center mb-8">
            <div>
              <h1 className="text-3xl font-bold tracking-tight sm:text-4xl mb-2">Seguimiento de Crecimiento de Clientes</h1>
              <p className="text-muted-foreground">
                Seguimiento y gestión de sus clientes y métricas de crecimiento
              </p>
            </div>
            
            <CustomerFormDialog 
              availableServices={services.map(service => service.name)}
              onAddCustomer={handleAddCustomer}
            />
          </div>
          
          <div className="grid grid-cols-1 gap-8">
            <ServiceUsageChart 
              serviceUsageData={serviceUsageData} 
              isLoading={isLoading} 
              calculateBrandGrowth={calculateBrandGrowth}
              calculateMonthlyGrowth={calculateMonthlyGrowth}
              calculateMonthlyGrowthByBrand={calculateMonthlyGrowthByBrand}
            />
            <CustomerTable customers={customers} isLoading={isLoading} />
          </div>
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
