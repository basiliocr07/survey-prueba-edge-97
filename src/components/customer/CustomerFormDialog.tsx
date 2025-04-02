
import { useState } from 'react';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Customer } from '@/domain/models/Customer';
import { PlusCircle } from 'lucide-react';
import { Checkbox } from '@/components/ui/checkbox';
import { useToast } from '@/components/ui/use-toast';

interface CustomerFormDialogProps {
  availableServices: string[];
  onAddCustomer: (customer: Customer) => void;
}

export default function CustomerFormDialog({ availableServices, onAddCustomer }: CustomerFormDialogProps) {
  const { toast } = useToast();
  const [open, setOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formData, setFormData] = useState<Partial<Customer>>({
    brand_name: '',
    contact_name: '',
    contact_email: '',
    contact_phone: '',
    acquired_services: [],
    customer_type: 'client' // Valor por defecto
  });
  
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const toggleService = (service: string) => {
    setFormData(prev => {
      const currentServices = prev.acquired_services || [];
      if (currentServices.includes(service)) {
        return {
          ...prev,
          acquired_services: currentServices.filter(s => s !== service)
        };
      } else {
        return {
          ...prev,
          acquired_services: [...currentServices, service]
        };
      }
    });
  };

  const handleSelectChange = (name: string, value: string) => {
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!formData.brand_name || !formData.contact_name || !formData.contact_email) {
      toast({
        title: "Error en el formulario",
        description: "Por favor complete todos los campos requeridos.",
        variant: "destructive"
      });
      return;
    }
    
    setIsSubmitting(true);
    
    try {
      // Create new customer object with real data
      const newCustomer: Customer = {
        id: '0', // Will be replaced by the backend
        brand_name: formData.brand_name || '',
        contact_name: formData.contact_name || '',
        contact_email: formData.contact_email || '',
        contact_phone: formData.contact_phone,
        acquired_services: formData.acquired_services || [],
        customer_type: formData.customer_type as 'admin' | 'client' || 'client',
        created_at: new Date().toISOString(),
        updated_at: new Date().toISOString()
      };
      
      await onAddCustomer(newCustomer);
      
      toast({
        title: "Cliente añadido",
        description: "El cliente ha sido creado exitosamente",
      });
      
      // Reset form and close dialog
      setFormData({
        brand_name: '',
        contact_name: '',
        contact_email: '',
        contact_phone: '',
        acquired_services: [],
        customer_type: 'client'
      });
      setOpen(false);
    } catch (error) {
      console.error("Error adding customer:", error);
      toast({
        title: "Error",
        description: "No se pudo añadir el cliente. Por favor intente nuevamente.",
        variant: "destructive"
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button className="gap-2">
          <PlusCircle className="h-4 w-4" />
          Añadir Cliente
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[425px]">
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle>Añadir Nuevo Cliente</DialogTitle>
            <DialogDescription>
              Ingrese los datos del cliente y los servicios que ha adquirido.
            </DialogDescription>
          </DialogHeader>
          <div className="grid gap-4 py-4">
            <div className="grid gap-2">
              <Label htmlFor="brand_name">Nombre de Marca</Label>
              <Input
                id="brand_name"
                name="brand_name"
                value={formData.brand_name}
                onChange={handleInputChange}
                required
              />
            </div>
            <div className="grid gap-2">
              <Label htmlFor="contact_name">Persona de Contacto</Label>
              <Input
                id="contact_name"
                name="contact_name"
                value={formData.contact_name}
                onChange={handleInputChange}
                required
              />
            </div>
            <div className="grid gap-2">
              <Label htmlFor="contact_email">Email de Contacto</Label>
              <Input
                id="contact_email"
                name="contact_email"
                type="email"
                value={formData.contact_email}
                onChange={handleInputChange}
                required
              />
            </div>
            <div className="grid gap-2">
              <Label htmlFor="contact_phone">Teléfono de Contacto</Label>
              <Input
                id="contact_phone"
                name="contact_phone"
                value={formData.contact_phone}
                onChange={handleInputChange}
              />
            </div>
            <div className="grid gap-2">
              <Label htmlFor="customer_type">Tipo de Cliente</Label>
              <Select 
                name="customer_type"
                value={formData.customer_type || 'client'}
                onValueChange={(value) => handleSelectChange('customer_type', value)}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Seleccionar tipo de cliente" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="client">Cliente</SelectItem>
                  <SelectItem value="admin">Administrador</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="grid gap-2">
              <Label>Servicios Adquiridos</Label>
              <div className="grid grid-cols-2 gap-2">
                {availableServices.map(service => (
                  <div key={service} className="flex items-center space-x-2">
                    <Checkbox 
                      id={`service-${service}`}
                      checked={(formData.acquired_services || []).includes(service)}
                      onCheckedChange={() => toggleService(service)}
                    />
                    <Label htmlFor={`service-${service}`} className="text-sm">
                      {service}
                    </Label>
                  </div>
                ))}
              </div>
            </div>
          </div>
          <DialogFooter>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? "Añadiendo..." : "Añadir Cliente"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
