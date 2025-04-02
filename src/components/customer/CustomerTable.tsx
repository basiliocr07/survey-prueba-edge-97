
import { format } from 'date-fns';
import { User, ChevronRight } from 'lucide-react';
import { Customer } from '@/domain/models/Customer';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { 
  Table,
  TableHeader,
  TableRow,
  TableHead,
  TableBody,
  TableCell
} from '@/components/ui/table';

interface CustomerTableProps {
  customers: Customer[];
  isLoading?: boolean;
}

export default function CustomerTable({ customers, isLoading }: CustomerTableProps) {
  if (isLoading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <User className="h-5 w-5" />
            Directorio de Clientes
          </CardTitle>
          <CardDescription>
            Cargando datos de clientes...
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="h-[200px] flex items-center justify-center">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
          </div>
        </CardContent>
      </Card>
    );
  }

  if (!customers.length) {
    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <User className="h-5 w-5" />
            Directorio de Clientes
          </CardTitle>
          <CardDescription>
            No se encontraron clientes
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="h-[200px] flex items-center justify-center">
            <p className="text-muted-foreground">No hay clientes para mostrar</p>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <User className="h-5 w-5" />
          Directorio de Clientes
        </CardTitle>
        <CardDescription>
          Lista de todos los clientes y su información
        </CardDescription>
      </CardHeader>
      <CardContent>
        <div className="overflow-x-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Marca</TableHead>
                <TableHead>Contacto</TableHead>
                <TableHead>Tipo</TableHead>
                <TableHead>Servicios</TableHead>
                <TableHead>Desde</TableHead>
                <TableHead>Acciones</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {customers.map(customer => (
                <TableRow key={customer.id}>
                  <TableCell className="font-medium">{customer.brand_name}</TableCell>
                  <TableCell>
                    <div>{customer.contact_email}</div>
                    <div className="text-muted-foreground text-sm">{customer.contact_phone}</div>
                  </TableCell>
                  <TableCell>
                    <Badge variant={customer.customer_type === 'admin' ? 'destructive' : 'default'}>
                      {customer.customer_type === 'admin' ? 'Admin' : 'Cliente'}
                    </Badge>
                  </TableCell>
                  <TableCell>
                    <div className="flex flex-wrap gap-1">
                      {Array.isArray(customer.acquired_services) && customer.acquired_services.map(service => (
                        <span key={service} className="inline-block px-2 py-1 text-xs bg-primary/10 text-primary rounded-full">
                          {service}
                        </span>
                      ))}
                    </div>
                  </TableCell>
                  <TableCell>{format(new Date(customer.created_at), 'MMM yyyy')}</TableCell>
                  <TableCell>
                    <Button variant="ghost" size="sm">
                      <ChevronRight className="h-4 w-4" />
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      </CardContent>
    </Card>
  );
}
