
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
            Customer Directory
          </CardTitle>
          <CardDescription>
            Loading customer data...
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="h-[200px] flex items-center justify-center">
            <p className="text-muted-foreground">Loading customers...</p>
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
            Customer Directory
          </CardTitle>
          <CardDescription>
            No customers found
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="h-[200px] flex items-center justify-center">
            <p className="text-muted-foreground">No customers to display</p>
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
          Customer Directory
        </CardTitle>
        <CardDescription>
          List of all customers and their information
        </CardDescription>
      </CardHeader>
      <CardContent>
        <div className="overflow-x-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Brand Name</TableHead>
                <TableHead>Contact</TableHead>
                <TableHead>Type</TableHead>
                <TableHead>Services</TableHead>
                <TableHead>Since</TableHead>
                <TableHead>Actions</TableHead>
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
                      {customer.customer_type === 'admin' ? 'Admin' : 'Client'}
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
