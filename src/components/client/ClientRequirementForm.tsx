
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { Search, CheckCircle2, AlertTriangle } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/hooks/use-toast';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Suggestion, KnowledgeBaseItem } from '@/types/suggestions';

interface RequirementFormData {
  title: string;
  description: string;
  customerName: string;
  customerEmail: string;
  priority: string;
  projectArea?: string;
}

const priorities = [
  'Alta',
  'Media',
  'Baja'
];

const projectAreas = [
  'Frontend',
  'Backend',
  'UI/UX',
  'Base de datos',
  'General'
];

export default function ClientRequirementForm() {
  const [isSearching, setIsSearching] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [searchResults, setSearchResults] = useState<{ requirements: Suggestion[], knowledgeItems: KnowledgeBaseItem[] }>({ requirements: [], knowledgeItems: [] });
  const { toast } = useToast();
  const { register, handleSubmit, reset, watch, formState: { errors } } = useForm<RequirementFormData>({
    defaultValues: {
      priority: 'Media',
      projectArea: 'General'
    }
  });
  
  const requirementContent = watch('description');

  const handleSearch = (content: string) => {
    if (!content || content.length < 10) return;
    
    // Este es un ejemplo simplificado para la interfaz del cliente
    // En una aplicación real, esto llamaría a un endpoint de API para buscar elementos similares
    setIsSearching(true);
    
    // Resultados simulados de búsqueda
    setSearchResults({
      requirements: [],
      knowledgeItems: []
    });
  };

  const onSubmit = async (data: RequirementFormData) => {
    try {
      setIsSubmitting(true);
      // Enviar los datos al endpoint de API
      const response = await fetch('/api/requirements', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          title: data.title,
          description: data.description,
          priority: data.priority,
          projectArea: data.projectArea || 'General',
          customerName: data.customerName,
          customerEmail: data.customerEmail
        })
      });
      
      if (response.ok) {
        toast({
          title: "Requerimiento enviado",
          description: "Tu requerimiento ha sido registrado correctamente.",
          variant: "default"
        });
        reset();
        setIsSearching(false);
        setSearchResults({ requirements: [], knowledgeItems: [] });
      } else {
        const errorData = await response.json();
        toast({
          title: "Error",
          description: errorData.message || "Hubo un problema al enviar tu requerimiento. Por favor, intenta de nuevo.",
          variant: "destructive"
        });
      }
    } catch (error) {
      console.error("Error al enviar el requerimiento:", error);
      toast({
        title: "Error de conexión",
        description: "No pudimos conectar con el servidor. Verifica tu conexión e intenta de nuevo.",
        variant: "destructive"
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div className="space-y-2">
        <Label htmlFor="title">Título del Requerimiento</Label>
        <Input 
          id="title" 
          placeholder="Resume tu requerimiento en un título breve" 
          {...register('title', { required: "El título es obligatorio" })}
        />
        {errors.title && (
          <p className="text-sm text-destructive">{errors.title.message}</p>
        )}
      </div>

      <div className="space-y-2">
        <Label htmlFor="description">Descripción del Requerimiento</Label>
        <Textarea 
          id="description" 
          placeholder="Describe tu requerimiento en detalle..." 
          className="min-h-[120px]"
          {...register('description', { required: "La descripción es obligatoria" })}
          onChange={(e) => {
            if (e.target.value.length > 15) {
              handleSearch(e.target.value);
            }
          }}
        />
        {errors.description && (
          <p className="text-sm text-destructive">{errors.description.message}</p>
        )}
      </div>
      
      {isSearching && (searchResults.requirements.length > 0 || searchResults.knowledgeItems.length > 0) && (
        <div className="bg-muted p-4 rounded-md border border-border">
          <h4 className="font-medium flex items-center gap-2 mb-2">
            <Search className="h-4 w-4" />
            Se encontraron elementos similares
          </h4>
          
          {searchResults.requirements.length > 0 && (
            <div className="mb-2">
              <p className="text-sm font-medium mb-1">Requerimientos similares:</p>
              <ul className="text-sm space-y-1">
                {searchResults.requirements.map((req) => (
                  <li key={req.id.toString()} className="flex items-start gap-2">
                    <span className="text-primary text-xs mt-0.5">•</span>
                    <span>{req.content}</span>
                  </li>
                ))}
              </ul>
            </div>
          )}
          
          {searchResults.knowledgeItems.length > 0 && (
            <div>
              <p className="text-sm font-medium mb-1">Artículos relacionados:</p>
              <ul className="text-sm space-y-1">
                {searchResults.knowledgeItems.map((item) => (
                  <li key={item.id.toString()} className="flex items-start gap-2">
                    <span className="text-primary text-xs mt-0.5">•</span>
                    <span>{item.title}</span>
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}
      
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="space-y-2">
          <Label htmlFor="customerName">Tu Nombre</Label>
          <Input 
            id="customerName" 
            placeholder="John Doe" 
            {...register('customerName', { required: "El nombre es obligatorio" })}
          />
          {errors.customerName && (
            <p className="text-sm text-destructive">{errors.customerName.message}</p>
          )}
        </div>
        
        <div className="space-y-2">
          <Label htmlFor="customerEmail">Correo Electrónico</Label>
          <Input 
            id="customerEmail" 
            type="email" 
            placeholder="john@example.com" 
            {...register('customerEmail', { 
              required: "El correo es obligatorio",
              pattern: {
                value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                message: "Correo electrónico inválido"
              }
            })}
          />
          {errors.customerEmail && (
            <p className="text-sm text-destructive">{errors.customerEmail.message}</p>
          )}
        </div>
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="space-y-2">
          <Label htmlFor="priority">Prioridad</Label>
          <select 
            id="priority"
            className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-base ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium file:text-foreground placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 md:text-sm"
            {...register('priority', { required: "La prioridad es obligatoria" })}
          >
            <option value="">Selecciona la prioridad</option>
            {priorities.map((priority) => (
              <option key={priority} value={priority}>{priority}</option>
            ))}
          </select>
          {errors.priority && (
            <p className="text-sm text-destructive">{errors.priority.message}</p>
          )}
        </div>
        
        <div className="space-y-2">
          <Label htmlFor="projectArea">Área del Proyecto</Label>
          <select 
            id="projectArea"
            className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-base ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium file:text-foreground placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 md:text-sm"
            {...register('projectArea')}
          >
            <option value="">Selecciona un área</option>
            {projectAreas.map((area) => (
              <option key={area} value={area}>{area}</option>
            ))}
          </select>
        </div>
      </div>
      
      <Button type="submit" className="w-full" disabled={isSubmitting}>
        {isSubmitting ? 'Enviando...' : 'Enviar Requerimiento'}
      </Button>
    </form>
  );
}
