import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Separator } from "@/components/ui/separator";
import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Switch } from "@/components/ui/switch";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { CalendarIcon, Clock, Mail, Ticket, Calendar as CalendarIcon2, Check, X, UserPlus, Users, ChevronDown, ChevronUp } from "lucide-react";
import { format } from "date-fns";
import { cn } from "@/lib/utils";
import { Checkbox } from "@/components/ui/checkbox";
import { useToast } from "@/hooks/use-toast";

const mockCustomers = [
  { id: '1', email: 'customer1@example.com', name: 'John Doe' },
  { id: '2', email: 'customer2@example.com', name: 'Jane Smith' },
  { id: '3', email: 'customer3@example.com', name: 'Robert Johnson' },
  { id: '4', email: 'customer4@example.com', name: 'Lisa Brown' },
  { id: '5', email: 'customer5@example.com', name: 'Michael Wilson' },
  { id: '6', email: 'customer6@example.com', name: 'Sarah Davis' },
  { id: '7', email: 'customer7@example.com', name: 'James Miller' },
  { id: '8', email: 'customer8@example.com', name: 'Emily Taylor' },
];

export type DeliveryConfig = {
  type: 'manual' | 'scheduled' | 'triggered';
  emailAddresses: string[];
  schedule?: {
    frequency: 'daily' | 'weekly' | 'monthly';
    dayOfMonth?: number;
    dayOfWeek?: number;
    time?: string;
    startDate?: Date;
  };
  trigger?: {
    type: 'ticket-closed' | 'purchase-completed';
    delayHours: number;
    sendAutomatically: boolean;
  };
};

interface EmailDeliverySettingsProps {
  deliveryConfig: DeliveryConfig;
  onConfigChange: (config: DeliveryConfig) => void;
}

export default function EmailDeliverySettings({ deliveryConfig, onConfigChange }: EmailDeliverySettingsProps) {
  const { toast } = useToast();
  const [emailInput, setEmailInput] = useState<string>('');
  const [date, setDate] = useState<Date | undefined>(deliveryConfig.schedule?.startDate);
  const [showCustomerSelector, setShowCustomerSelector] = useState<boolean>(false);
  const [selectedCustomers, setSelectedCustomers] = useState<string[]>(deliveryConfig.emailAddresses);
  const [filterText, setFilterText] = useState<string>('');
  const [showSelectedRecipients, setShowSelectedRecipients] = useState<boolean>(false);
  const [isSending, setIsSending] = useState<boolean>(false);
  
  useEffect(() => {
    if (!deliveryConfig.trigger?.hasOwnProperty('sendAutomatically')) {
      onConfigChange({
        ...deliveryConfig,
        trigger: {
          ...deliveryConfig.trigger,
          sendAutomatically: false,
        }
      });
    }
  }, []);

  useEffect(() => {
    setSelectedCustomers(deliveryConfig.emailAddresses);
  }, [deliveryConfig.emailAddresses]);

  const handleDeliveryTypeChange = (value: 'manual' | 'scheduled' | 'triggered') => {
    onConfigChange({
      ...deliveryConfig,
      type: value,
    });
  };

  const handleFrequencyChange = (value: 'daily' | 'weekly' | 'monthly') => {
    onConfigChange({
      ...deliveryConfig,
      schedule: {
        ...deliveryConfig.schedule,
        frequency: value,
      },
    });
  };

  const handleTriggerTypeChange = (value: 'ticket-closed' | 'purchase-completed') => {
    onConfigChange({
      ...deliveryConfig,
      trigger: {
        ...deliveryConfig.trigger,
        type: value,
      },
    });
  };

  const handleDelayHoursChange = (value: string) => {
    onConfigChange({
      ...deliveryConfig,
      trigger: {
        ...deliveryConfig.trigger,
        delayHours: parseInt(value) || 0,
      },
    });
  };

  const handleDateChange = (date: Date | undefined) => {
    setDate(date);
    if (date) {
      onConfigChange({
        ...deliveryConfig,
        schedule: {
          ...deliveryConfig.schedule,
          startDate: date,
        },
      });
    }
  };

  const handleTimeChange = (value: string) => {
    onConfigChange({
      ...deliveryConfig,
      schedule: {
        ...deliveryConfig.schedule,
        time: value,
      },
    });
  };

  const handleDayChange = (value: string) => {
    const day = parseInt(value);
    if (deliveryConfig.schedule?.frequency === 'monthly') {
      onConfigChange({
        ...deliveryConfig,
        schedule: {
          ...deliveryConfig.schedule,
          dayOfMonth: day,
        },
      });
    } else if (deliveryConfig.schedule?.frequency === 'weekly') {
      onConfigChange({
        ...deliveryConfig,
        schedule: {
          ...deliveryConfig.schedule,
          dayOfWeek: day,
        },
      });
    }
  };

  const toggleAutomaticSending = () => {
    onConfigChange({
      ...deliveryConfig,
      trigger: {
        ...deliveryConfig.trigger,
        sendAutomatically: !deliveryConfig.trigger?.sendAutomatically,
      }
    });

    if (!deliveryConfig.trigger?.sendAutomatically) {
      toast({
        title: "Automatic sending enabled",
        description: `Surveys will be sent automatically when a ${deliveryConfig.trigger?.type === 'ticket-closed' ? 'ticket is closed' : 'purchase is completed'}.`,
      });
    }
  };

  const addEmail = () => {
    if (emailInput && !deliveryConfig.emailAddresses.includes(emailInput)) {
      onConfigChange({
        ...deliveryConfig,
        emailAddresses: [...deliveryConfig.emailAddresses, emailInput],
      });
      setEmailInput('');
    }
  };

  const removeEmail = (email: string) => {
    onConfigChange({
      ...deliveryConfig,
      emailAddresses: deliveryConfig.emailAddresses.filter(e => e !== email),
    });
  };

  const selectAllCustomers = () => {
    const allEmails = mockCustomers.map(customer => customer.email);
    onConfigChange({
      ...deliveryConfig,
      emailAddresses: allEmails,
    });
    setSelectedCustomers(allEmails);
  };

  const deselectAllCustomers = () => {
    onConfigChange({
      ...deliveryConfig,
      emailAddresses: [],
    });
    setSelectedCustomers([]);
  };

  const toggleCustomer = (email: string) => {
    if (selectedCustomers.includes(email)) {
      const updatedSelection = selectedCustomers.filter(e => e !== email);
      setSelectedCustomers(updatedSelection);
      onConfigChange({
        ...deliveryConfig,
        emailAddresses: updatedSelection,
      });
    } else {
      const updatedSelection = [...selectedCustomers, email];
      setSelectedCustomers(updatedSelection);
      onConfigChange({
        ...deliveryConfig,
        emailAddresses: updatedSelection,
      });
    }
  };

  const filteredCustomers = mockCustomers.filter(customer => 
    customer.name.toLowerCase().includes(filterText.toLowerCase()) || 
    customer.email.toLowerCase().includes(filterText.toLowerCase())
  );

  const sendSurveyEmails = async () => {
    if (deliveryConfig.emailAddresses.length === 0) {
      toast({
        title: "No hay destinatarios",
        description: "Por favor, añade al menos una dirección de correo electrónico",
        variant: "destructive"
      });
      return;
    }

    setIsSending(true);
    
    try {
      // Aquí se implementaría la conexión al backend
      // Este es un ejemplo simulado
      toast({
        title: "Emails en cola",
        description: `La encuesta será enviada a ${deliveryConfig.emailAddresses.length} destinatario(s) según la configuración de envío`,
      });

      console.log("Configuración de envío de correo:", deliveryConfig);
      
      // Esperar 2 segundos para simular el envío
      setTimeout(() => {
        toast({
          title: "Envío completado",
          description: "Los correos han sido procesados correctamente",
        });
        setIsSending(false);
      }, 2000);
    } catch (error) {
      console.error("Error al enviar emails:", error);
      toast({
        title: "Error al enviar",
        description: "Ocurrió un problema al procesar los correos electrónicos",
        variant: "destructive"
      });
      setIsSending(false);
    }
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle>Configuración de Envío por Email</CardTitle>
      </CardHeader>
      <CardContent className="space-y-6">
        <div>
          <h3 className="text-lg font-medium mb-4">Delivery Method</h3>
          <RadioGroup
            value={deliveryConfig.type}
            onValueChange={(value) => handleDeliveryTypeChange(value as 'manual' | 'scheduled' | 'triggered')}
            className="space-y-3"
          >
            <div className="flex items-start space-x-2">
              <RadioGroupItem value="manual" id="manual" />
              <div className="grid gap-1.5">
                <Label htmlFor="manual" className="font-medium">Manual Sending</Label>
                <p className="text-sm text-muted-foreground">
                  Send surveys manually when needed
                </p>
              </div>
            </div>
            
            <div className="flex items-start space-x-2">
              <RadioGroupItem value="scheduled" id="scheduled" />
              <div className="grid gap-1.5">
                <Label htmlFor="scheduled" className="font-medium">Scheduled</Label>
                <p className="text-sm text-muted-foreground">
                  Send surveys on a regular schedule (daily, weekly, monthly)
                </p>
              </div>
            </div>
            
            <div className="flex items-start space-x-2">
              <RadioGroupItem value="triggered" id="triggered" />
              <div className="grid gap-1.5">
                <Label htmlFor="triggered" className="font-medium">Event Triggered</Label>
                <p className="text-sm text-muted-foreground">
                  Send surveys when specific events occur (ticket closed, purchase completed)
                </p>
              </div>
            </div>
          </RadioGroup>
        </div>
        
        <Separator />
        
        <div>
          <h3 className="text-lg font-medium mb-4">Recipient Emails</h3>
          <div className="flex space-x-2 mb-4">
            <Input
              placeholder="Add email address"
              value={emailInput}
              onChange={(e) => setEmailInput(e.target.value)}
              className="flex-1"
            />
            <Button onClick={addEmail} type="button">Add</Button>
          </div>

          <div className="flex space-x-2 mb-4">
            <Button 
              onClick={() => setShowCustomerSelector(!showCustomerSelector)} 
              variant="outline" 
              className="flex-1"
            >
              <Users className="mr-2 h-4 w-4" />
              {showCustomerSelector ? "Hide Customer List" : "Select from Customers"}
            </Button>
          </div>
          
          {showCustomerSelector && (
            <div className="border rounded-md p-4 mb-4 bg-background">
              <div className="flex justify-between items-center mb-3">
                <h4 className="font-medium">Customer Selection</h4>
                <div className="flex space-x-2">
                  <Button onClick={selectAllCustomers} size="sm" variant="outline">
                    <Check className="mr-1 h-3 w-3" /> Select All
                  </Button>
                  <Button onClick={deselectAllCustomers} size="sm" variant="outline">
                    <X className="mr-1 h-3 w-3" /> Deselect All
                  </Button>
                </div>
              </div>
              
              <Input 
                placeholder="Filter customers..." 
                value={filterText} 
                onChange={(e) => setFilterText(e.target.value)}
                className="mb-3"
              />
              
              <div className="max-h-60 overflow-auto border rounded-md">
                {filteredCustomers.length > 0 ? (
                  filteredCustomers.map(customer => (
                    <div 
                      key={customer.id} 
                      className="flex items-center justify-between p-2 hover:bg-muted border-b last:border-0"
                    >
                      <div className="flex items-center space-x-2">
                        <Checkbox 
                          id={`customer-${customer.id}`}
                          checked={selectedCustomers.includes(customer.email)}
                          onCheckedChange={() => toggleCustomer(customer.email)}
                        />
                        <Label 
                          htmlFor={`customer-${customer.id}`}
                          className="cursor-pointer flex-1"
                        >
                          <div>{customer.name}</div>
                          <div className="text-xs text-muted-foreground">{customer.email}</div>
                        </Label>
                      </div>
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => toggleCustomer(customer.email)}
                      >
                        {selectedCustomers.includes(customer.email) ? (
                          <X className="h-4 w-4" />
                        ) : (
                          <UserPlus className="h-4 w-4" />
                        )}
                      </Button>
                    </div>
                  ))
                ) : (
                  <div className="p-3 text-center text-muted-foreground">
                    No customers match your search
                  </div>
                )}
              </div>
            </div>
          )}
          
          {deliveryConfig.emailAddresses.length > 0 && (
            <div className="border rounded-md p-4 mb-4 bg-background">
              <Button 
                variant="outline" 
                className="w-full flex justify-between items-center mb-3"
                onClick={() => setShowSelectedRecipients(!showSelectedRecipients)}
              >
                <span className="font-medium">Selected Recipients ({deliveryConfig.emailAddresses.length})</span>
                {showSelectedRecipients ? (
                  <ChevronUp className="h-4 w-4" />
                ) : (
                  <ChevronDown className="h-4 w-4" />
                )}
              </Button>
              
              {showSelectedRecipients && (
                <div className="space-y-2 mt-4 max-h-60 overflow-auto">
                  {deliveryConfig.emailAddresses.map((email, index) => (
                    <div key={index} className="flex items-center justify-between bg-muted p-2 rounded">
                      <span className="text-sm">{email}</span>
                      <Button variant="ghost" size="sm" onClick={() => removeEmail(email)}>
                        <X className="h-4 w-4" />
                      </Button>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}
          
          {deliveryConfig.emailAddresses.length === 0 && (
            <p className="text-sm text-muted-foreground mb-4">No recipients added yet</p>
          )}
        </div>
        
        {deliveryConfig.type === 'scheduled' && (
          <>
            <Separator />
            
            <div className="space-y-4">
              <h3 className="text-lg font-medium">Schedule Configuration</h3>
              
              <div className="space-y-4">
                <div>
                  <Label htmlFor="frequency">Frequency</Label>
                  <Select
                    value={deliveryConfig.schedule?.frequency || 'monthly'}
                    onValueChange={(value) => handleFrequencyChange(value as 'daily' | 'weekly' | 'monthly')}
                  >
                    <SelectTrigger id="frequency">
                      <SelectValue placeholder="Select frequency" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="daily">Daily</SelectItem>
                      <SelectItem value="weekly">Weekly</SelectItem>
                      <SelectItem value="monthly">Monthly</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                
                {deliveryConfig.schedule?.frequency === 'weekly' && (
                  <div>
                    <Label htmlFor="day-of-week">Day of Week</Label>
                    <Select
                      value={deliveryConfig.schedule?.dayOfWeek?.toString() || '1'}
                      onValueChange={handleDayChange}
                    >
                      <SelectTrigger id="day-of-week">
                        <SelectValue placeholder="Select day" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="1">Monday</SelectItem>
                        <SelectItem value="2">Tuesday</SelectItem>
                        <SelectItem value="3">Wednesday</SelectItem>
                        <SelectItem value="4">Thursday</SelectItem>
                        <SelectItem value="5">Friday</SelectItem>
                        <SelectItem value="6">Saturday</SelectItem>
                        <SelectItem value="0">Sunday</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                )}
                
                {deliveryConfig.schedule?.frequency === 'monthly' && (
                  <div>
                    <Label htmlFor="day-of-month">Day of Month</Label>
                    <Select
                      value={deliveryConfig.schedule?.dayOfMonth?.toString() || '1'}
                      onValueChange={handleDayChange}
                    >
                      <SelectTrigger id="day-of-month">
                        <SelectValue placeholder="Select day" />
                      </SelectTrigger>
                      <SelectContent>
                        {Array.from({ length: 31 }, (_, i) => (
                          <SelectItem key={i} value={(i + 1).toString()}>
                            {i + 1}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                )}
                
                <div>
                  <Label htmlFor="time">Time of Day</Label>
                  <Input
                    id="time"
                    type="time"
                    value={deliveryConfig.schedule?.time || '09:00'}
                    onChange={(e) => handleTimeChange(e.target.value)}
                  />
                </div>
                
                <div>
                  <Label>Start Date</Label>
                  <Popover>
                    <PopoverTrigger asChild>
                      <Button
                        variant="outline"
                        className={cn(
                          "w-full justify-start text-left font-normal",
                          !date && "text-muted-foreground"
                        )}
                      >
                        <CalendarIcon className="mr-2 h-4 w-4" />
                        {date ? format(date, "PPP") : "Pick a date"}
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent className="w-auto p-0">
                      <Calendar
                        mode="single"
                        selected={date}
                        onSelect={handleDateChange}
                        initialFocus
                      />
                    </PopoverContent>
                  </Popover>
                </div>
              </div>
            </div>
          </>
        )}
        
        {deliveryConfig.type === 'triggered' && (
          <>
            <Separator />
            
            <div className="space-y-4">
              <h3 className="text-lg font-medium">Trigger Configuration</h3>
              
              <div>
                <Label htmlFor="trigger-type">Trigger Event</Label>
                <Select
                  value={deliveryConfig.trigger?.type || 'ticket-closed'}
                  onValueChange={(value) => handleTriggerTypeChange(value as 'ticket-closed' | 'purchase-completed')}
                >
                  <SelectTrigger id="trigger-type">
                    <SelectValue placeholder="Select trigger" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="ticket-closed">Ticket Closed</SelectItem>
                    <SelectItem value="purchase-completed">Purchase Completed</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              
              <div>
                <Label htmlFor="delay">Delay (hours)</Label>
                <Input
                  id="delay"
                  type="number"
                  min="0"
                  value={deliveryConfig.trigger?.delayHours.toString() || '24'}
                  onChange={(e) => handleDelayHoursChange(e.target.value)}
                  placeholder="24"
                />
                <p className="text-xs text-muted-foreground mt-1">
                  Time to wait after the trigger event before sending the survey
                </p>
              </div>

              <div className="flex items-center space-x-2">
                <Switch 
                  id="auto-send"
                  checked={deliveryConfig.trigger?.sendAutomatically || false}
                  onCheckedChange={toggleAutomaticSending}
                />
                <Label htmlFor="auto-send" className="cursor-pointer">
                  <div className="font-medium">Send automatically</div>
                  <p className="text-xs text-muted-foreground">
                    Automatically send surveys when the {deliveryConfig.trigger?.type === 'ticket-closed' ? 'ticket is closed' : 'purchase is completed'}
                  </p>
                </Label>
              </div>
            </div>
          </>
        )}
        
        <div className="flex justify-end mt-4">
          <Button 
            onClick={sendSurveyEmails} 
            disabled={deliveryConfig.emailAddresses.length === 0 || isSending}
          >
            {isSending ? (
              <>
                <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Enviando...
              </>
            ) : (
              <>
                <Mail className="mr-2 h-4 w-4" /> Enviar Ahora
              </>
            )}
          </Button>
        </div>
      </CardContent>
    </Card>
  );
}
