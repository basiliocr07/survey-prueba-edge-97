
import { useState } from 'react';
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import ServiceUsageChart from '@/components/customer/ServiceUsageChart';
import CustomerTable from '@/components/customer/CustomerTable';
import CustomerFormDialog from '@/components/customer/CustomerFormDialog';
import UserFormDialog from '@/components/user/UserFormDialog';
import UserTable from '@/components/user/UserTable';
import { useCustomers } from '@/application/hooks/useCustomers';
import { useUsers } from '@/application/hooks/useUsers';
import { useToast } from '@/hooks/use-toast';
import { Customer } from '@/domain/models/Customer';
import { User } from '@/domain/models/User';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { supabase } from '@/integrations/supabase/client';
import { Users, BarChart3 } from 'lucide-react';

export default function CustomerGrowth() {
  const { customers, services, serviceUsageData, isLoading: isCustomersLoading, error: customersError } = useCustomers();
  const { users, isLoading: isUsersLoading, error: usersError } = useUsers();
  const { toast } = useToast();
  const [activeTab, setActiveTab] = useState('customers');

  // Handle customer form submission
  const handleAddCustomer = async (newCustomer: Customer) => {
    try {
      // Primero insertar el cliente
      const { data: insertedCustomer, error: customerError } = await supabase
        .from('customers')
        .insert({
          brand_name: newCustomer.brand_name,
          contact_name: newCustomer.contact_name,
          contact_email: newCustomer.contact_email,
          contact_phone: newCustomer.contact_phone
        })
        .select('id')
        .single();

      if (customerError || !insertedCustomer) throw customerError;

      // Luego crear las relaciones con los servicios
      if (newCustomer.acquired_services && newCustomer.acquired_services.length > 0) {
        // Obtener los IDs de los servicios por nombre
        const { data: serviceData, error: servicesError } = await supabase
          .from('services')
          .select('id, name')
          .in('name', newCustomer.acquired_services);
        
        if (servicesError || !serviceData) throw servicesError;
        
        // Crear las entradas en la tabla de relación
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

  // Handle user form submission
  const handleAddUser = async (newUser: User) => {
    try {
      const { error } = await supabase
        .from('users')
        .insert({
          username: newUser.username,
          email: newUser.email,
          full_name: newUser.fullName,
          role: newUser.role,
          active: true,
          created_at: new Date().toISOString(),
        });
      
      if (error) throw error;

      toast({
        title: "Usuario añadido",
        description: `El usuario ${newUser.username} ha sido añadido exitosamente como ${newUser.role}.`,
      });
    } catch (error) {
      console.error('Error adding user:', error);
      toast({
        title: "Error",
        description: "No se pudo añadir el usuario. Por favor intente nuevamente.",
        variant: "destructive",
      });
    }
  };

  // Show error message if data fetching failed
  const error = customersError || usersError;
  if (error) {
    return (
      <div className="min-h-screen flex flex-col bg-background">
        <Navbar />
        <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
          <div className="w-[900px] max-w-[900px] h-full bg-white p-6">
            <div className="text-center py-10">
              <h2 className="text-xl font-semibold text-destructive">Error cargando datos</h2>
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
              <h1 className="text-3xl font-bold tracking-tight sm:text-4xl mb-2">Gestión de Clientes y Usuarios</h1>
              <p className="text-muted-foreground">
                Seguimiento y gestión de sus clientes, usuarios y métricas de crecimiento
              </p>
            </div>
          </div>
          
          <Tabs value={activeTab} onValueChange={setActiveTab} className="mb-8">
            <TabsList className="grid grid-cols-2 w-[400px]">
              <TabsTrigger value="customers" className="flex items-center gap-2">
                <BarChart3 className="h-4 w-4" />
                Clientes
              </TabsTrigger>
              <TabsTrigger value="users" className="flex items-center gap-2">
                <Users className="h-4 w-4" />
                Usuarios
              </TabsTrigger>
            </TabsList>

            <TabsContent value="customers" className="mt-4">
              <div className="flex justify-end mb-6">
                <CustomerFormDialog 
                  availableServices={services.map(service => service.name)}
                  onAddCustomer={handleAddCustomer}
                />
              </div>
              <div className="grid grid-cols-1 gap-8">
                <ServiceUsageChart serviceUsageData={serviceUsageData} isLoading={isCustomersLoading} />
                <CustomerTable customers={customers} isLoading={isCustomersLoading} />
              </div>
            </TabsContent>

            <TabsContent value="users" className="mt-4">
              <div className="flex justify-end mb-6">
                <UserFormDialog onAddUser={handleAddUser} />
              </div>
              <div className="grid grid-cols-1 gap-8">
                <UserTable users={users} isLoading={isUsersLoading} />
              </div>
            </TabsContent>
          </Tabs>
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
