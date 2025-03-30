
import { useState, useEffect } from 'react';
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { 
  Table, 
  TableBody, 
  TableCell, 
  TableHead, 
  TableHeader, 
  TableRow 
} from "@/components/ui/table";
import { 
  Tabs, 
  TabsContent, 
  TabsList, 
  TabsTrigger 
} from "@/components/ui/tabs";
import { useToast } from "@/hooks/use-toast";
import Navbar from "@/components/layout/Navbar";
import { BarChart3, UserPlus, Edit, Trash2, UserCheck } from "lucide-react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Customer, Service } from "@/domain/models/Customer";

type UserType = "admin" | "client";

// Componente principal
export default function CustomerGrowth() {
  const [userType, setUserType] = useState<UserType>("client");
  const { toast } = useToast();
  const queryClient = useQueryClient();

  // Consultas para obtener datos
  const { data: customersData, isLoading: isLoadingCustomers } = useQuery({
    queryKey: ['customers', userType],
    queryFn: async () => {
      try {
        const response = await fetch(`/api/customers?userType=${userType}`);
        if (!response.ok) throw new Error('Error al cargar clientes');
        return await response.json();
      } catch (error) {
        toast({
          title: "Error",
          description: "No se pudieron cargar los datos de clientes",
          variant: "destructive"
        });
        return { customers: [], services: [], serviceUsageData: [] };
      }
    }
  });

  // Mutaciones para operaciones CRUD
  const addCustomerMutation = useMutation({
    mutationFn: async (newCustomer: any) => {
      const response = await fetch('/api/customers', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({...newCustomer, userType})
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Error al añadir cliente');
      }
      
      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      toast({
        title: "Cliente añadido",
        description: "El cliente ha sido añadido exitosamente"
      });
    },
    onError: (error) => {
      toast({
        title: "Error",
        description: error instanceof Error ? error.message : "Error al añadir cliente",
        variant: "destructive"
      });
    }
  });

  const updateCustomerMutation = useMutation({
    mutationFn: async (customer: any) => {
      const response = await fetch(`/api/customers/${customer.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({...customer, userType})
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Error al actualizar cliente');
      }
      
      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      toast({
        title: "Cliente actualizado",
        description: "El cliente ha sido actualizado exitosamente"
      });
    },
    onError: (error) => {
      toast({
        title: "Error",
        description: error instanceof Error ? error.message : "Error al actualizar cliente",
        variant: "destructive"
      });
    }
  });

  const deleteCustomerMutation = useMutation({
    mutationFn: async (id: string) => {
      const response = await fetch(`/api/customers/${id}?userType=${userType}`, {
        method: 'DELETE'
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Error al eliminar cliente');
      }
      
      return true;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      toast({
        title: "Cliente eliminado",
        description: "El cliente ha sido eliminado exitosamente"
      });
    },
    onError: (error) => {
      toast({
        title: "Error",
        description: error instanceof Error ? error.message : "Error al eliminar cliente",
        variant: "destructive"
      });
    }
  });

  // Renderizar componente
  return (
    <div className="min-h-screen bg-background">
      <Navbar />
      <div className="container mx-auto pt-20 pb-10 px-4 md:px-6">
        <div className="flex flex-col space-y-6">
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
            <div>
              <h1 className="text-3xl font-bold tracking-tight">Crecimiento de Clientes</h1>
              <p className="text-muted-foreground mt-1">Gestión y análisis de clientes y servicios</p>
            </div>
            
            <div className="flex items-center gap-4">
              <div className="flex items-center space-x-2">
                <span className="text-sm font-medium">Ver como:</span>
                <Tabs value={userType} onValueChange={(value) => setUserType(value as UserType)} className="w-[200px]">
                  <TabsList className="grid w-full grid-cols-2">
                    <TabsTrigger value="client" className="flex items-center gap-1">
                      <UserCheck className="h-4 w-4" />
                      Cliente
                    </TabsTrigger>
                    <TabsTrigger value="admin" className="flex items-center gap-1">
                      <UserCheck className="h-4 w-4" />
                      Admin
                    </TabsTrigger>
                  </TabsList>
                </Tabs>
              </div>
              
              <Button>
                <UserPlus className="mr-2 h-4 w-4" />
                Añadir Cliente
              </Button>
            </div>
          </div>
          
          {isLoadingCustomers ? (
            <div className="flex justify-center items-center p-12">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
            </div>
          ) : (
            <div className="grid grid-cols-1 gap-6">
              {/* Gráfico de uso de servicios */}
              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-lg font-medium flex items-center">
                    <BarChart3 className="mr-2 h-5 w-5" />
                    Uso de Servicios
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="h-80">
                    {customersData?.serviceUsageData && 
                      <ServiceUsageChartComponent data={customersData.serviceUsageData} userType={userType} />
                    }
                  </div>
                </CardContent>
              </Card>
              
              {/* Tabla de clientes */}
              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-lg font-medium flex items-center">
                    <UserCheck className="mr-2 h-5 w-5" />
                    Clientes ({userType === "admin" ? "Administrador" : "Cliente"})
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Empresa</TableHead>
                        <TableHead>Contacto</TableHead>
                        <TableHead>Email</TableHead>
                        <TableHead>Teléfono</TableHead>
                        <TableHead>Servicios</TableHead>
                        {userType === "admin" && <TableHead className="text-right">Acciones</TableHead>}
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {customersData?.customers?.map((customer: any) => (
                        <TableRow key={customer.id}>
                          <TableCell className="font-medium">{customer.brandName}</TableCell>
                          <TableCell>{customer.contactName}</TableCell>
                          <TableCell>{customer.contactEmail}</TableCell>
                          <TableCell>{customer.contactPhone || "-"}</TableCell>
                          <TableCell>
                            <div className="flex flex-wrap gap-1">
                              {customer.services?.map((service: any) => (
                                <span key={service.id} className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-primary/10 text-primary">
                                  {service.name}
                                </span>
                              ))}
                            </div>
                          </TableCell>
                          {userType === "admin" && (
                            <TableCell className="text-right">
                              <Button variant="ghost" size="icon" className="h-8 w-8" onClick={() => {}}>
                                <Edit className="h-4 w-4" />
                              </Button>
                              <Button variant="ghost" size="icon" className="h-8 w-8 text-destructive" onClick={() => {}}>
                                <Trash2 className="h-4 w-4" />
                              </Button>
                            </TableCell>
                          )}
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </CardContent>
              </Card>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

// Componente para gráfico de uso de servicios
function ServiceUsageChartComponent({ data, userType }: { data: any[], userType: UserType }) {
  // Implementación del gráfico con recharts o similar
  return (
    <div className="text-center text-muted-foreground">
      Visualización de uso de servicios para {userType}
    </div>
  );
}
