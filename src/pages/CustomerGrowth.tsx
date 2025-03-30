
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

// List of available services for the form
const availableServices = [
  'Basic Survey',
  'Premium Survey',
  'Analytics Dashboard',
  'API Access',
  'Custom Branding',
  'Advanced Reports',
  'Email Marketing',
  'Data Export'
];

export default function CustomerGrowth() {
  const { customers, serviceUsageData, isLoading, error } = useCustomers();
  const { toast } = useToast();

  // Handle form submission
  const handleAddCustomer = async (newCustomer: Customer) => {
    try {
      const { error } = await supabase
        .from('customers')
        .insert({
          brand_name: newCustomer.brand_name,
          contact_name: newCustomer.contact_name,
          contact_email: newCustomer.contact_email,
          contact_phone: newCustomer.contact_phone,
          acquired_services: newCustomer.acquired_services
        });

      if (error) throw error;

      toast({
        title: "Customer added",
        description: "The customer has been successfully added.",
      });
    } catch (error) {
      console.error('Error adding customer:', error);
      toast({
        title: "Error",
        description: "Failed to add customer. Please try again.",
        variant: "destructive",
      });
    }
  };

  // Show error message if data fetching failed
  if (error) {
    return (
      <div className="min-h-screen flex flex-col bg-background">
        <Navbar />
        <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
          <div className="w-[900px] max-w-[900px] h-full bg-white p-6">
            <div className="text-center py-10">
              <h2 className="text-xl font-semibold text-destructive">Error loading customer data</h2>
              <p className="mt-2 text-muted-foreground">Please try again later or contact support.</p>
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
              <h1 className="text-3xl font-bold tracking-tight sm:text-4xl mb-2">Customer Growth Tracking</h1>
              <p className="text-muted-foreground">
                Track and manage your customers and their growth metrics
              </p>
            </div>
            
            <CustomerFormDialog 
              availableServices={availableServices}
              onAddCustomer={handleAddCustomer}
            />
          </div>
          
          <div className="grid grid-cols-1 gap-8">
            <ServiceUsageChart serviceUsageData={serviceUsageData} isLoading={isLoading} />
            <CustomerTable customers={customers} isLoading={isLoading} />
          </div>
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
