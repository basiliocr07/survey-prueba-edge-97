
import React, { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from "@/components/ui/command";
import { Calendar } from "@/components/ui/calendar";
import { Input } from "@/components/ui/input";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { Switch } from "@/components/ui/switch";
import { Check, Calendar as CalendarIcon, X, ChevronsUpDown, Search, CheckCircle, XCircle } from 'lucide-react';
import { cn } from '@/lib/utils';
import { DeliveryConfig } from '@/types/surveyTypes';
import { useCustomers } from '@/application/hooks/useCustomers';

interface EmailDeliverySettingsProps {
  deliveryConfig: DeliveryConfig;
  onConfigChange: (config: DeliveryConfig) => void;
}

export default function EmailDeliverySettings({ deliveryConfig, onConfigChange }: EmailDeliverySettingsProps) {
  const { customerEmails = [], isLoading } = useCustomers();
  const [emailInput, setEmailInput] = useState('');
  const [commandOpen, setCommandOpen] = useState(false);
  
  // Ensure emailAddresses is always an array
  useEffect(() => {
    if (!deliveryConfig.emailAddresses) {
      onConfigChange({
        ...deliveryConfig,
        emailAddresses: []
      });
    }
  }, [deliveryConfig, onConfigChange]);

  const handleAddEmail = () => {
    if (!emailInput.trim() || !isValidEmail(emailInput)) return;
    
    const email = emailInput.trim();
    const updatedEmails = [...(deliveryConfig.emailAddresses || [])];
    
    if (!updatedEmails.includes(email)) {
      updatedEmails.push(email);
      onConfigChange({
        ...deliveryConfig,
        emailAddresses: updatedEmails
      });
    }
    
    setEmailInput('');
  };

  const handleRemoveEmail = (email: string) => {
    const updatedEmails = (deliveryConfig.emailAddresses || []).filter(e => e !== email);
    onConfigChange({
      ...deliveryConfig,
      emailAddresses: updatedEmails
    });
  };

  const handleSelectCustomerEmail = (email: string) => {
    if (!email) return;
    
    const updatedEmails = [...(deliveryConfig.emailAddresses || [])];
    
    if (!updatedEmails.includes(email)) {
      updatedEmails.push(email);
      onConfigChange({
        ...deliveryConfig,
        emailAddresses: updatedEmails
      });
    }
    
    setCommandOpen(false);
  };

  const handleTypeChange = (type: 'manual' | 'scheduled' | 'triggered') => {
    onConfigChange({
      ...deliveryConfig,
      type,
      // Initialize sub-settings if needed
      ...(type === 'scheduled' && !deliveryConfig.schedule ? {
        schedule: { frequency: 'weekly', dayOfWeek: 1, time: '09:00' }
      } : {}),
      ...(type === 'triggered' && !deliveryConfig.trigger ? {
        trigger: { type: 'ticket-closed', delayHours: 24, sendAutomatically: true }
      } : {})
    });
  };

  // Función para seleccionar todos los correos electrónicos de clientes
  const handleSelectAllEmails = () => {
    if (!Array.isArray(customerEmails) || customerEmails.length === 0) return;
    
    // Combinamos los correos ya seleccionados con todos los correos de clientes
    const allEmails = [...(deliveryConfig.emailAddresses || [])];
    
    // Añadimos solo los correos que no están ya incluidos
    customerEmails.forEach(email => {
      if (!allEmails.includes(email)) {
        allEmails.push(email);
      }
    });
    
    onConfigChange({
      ...deliveryConfig,
      emailAddresses: allEmails
    });
  };

  // Función para deseleccionar todos los correos electrónicos
  const handleDeselectAllEmails = () => {
    onConfigChange({
      ...deliveryConfig,
      emailAddresses: []
    });
  };

  const isValidEmail = (email: string) => {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle>Delivery Settings</CardTitle>
      </CardHeader>
      <CardContent className="space-y-6">
        <div>
          <h3 className="text-lg font-medium mb-3">Delivery Method</h3>
          <Tabs 
            defaultValue={deliveryConfig.type || 'manual'} 
            onValueChange={(value) => handleTypeChange(value as 'manual' | 'scheduled' | 'triggered')}
            className="w-full"
          >
            <TabsList className="grid grid-cols-3 mb-4">
              <TabsTrigger value="manual">Manual</TabsTrigger>
              <TabsTrigger value="scheduled">Scheduled</TabsTrigger>
              <TabsTrigger value="triggered">Triggered</TabsTrigger>
            </TabsList>
            
            <TabsContent value="manual" className="space-y-4">
              <p className="text-muted-foreground">
                Send this survey manually to specific email addresses.
              </p>
            </TabsContent>
            
            <TabsContent value="scheduled" className="space-y-4">
              <p className="text-muted-foreground mb-4">
                Schedule this survey to be sent automatically at regular intervals.
              </p>
              
              <div className="space-y-4">
                <div>
                  <Label className="mb-2 block">Frequency</Label>
                  <RadioGroup 
                    value={deliveryConfig.schedule?.frequency || 'weekly'} 
                    onValueChange={(value) => {
                      const currentSchedule = deliveryConfig.schedule || { 
                        frequency: 'weekly', 
                        time: '09:00' 
                      };
                      onConfigChange({
                        ...deliveryConfig,
                        schedule: {
                          ...currentSchedule,
                          frequency: value as 'daily' | 'weekly' | 'monthly'
                        }
                      });
                    }}
                    className="flex flex-col space-y-2"
                  >
                    <div className="flex items-center space-x-2">
                      <RadioGroupItem value="daily" id="daily" />
                      <Label htmlFor="daily">Daily</Label>
                    </div>
                    <div className="flex items-center space-x-2">
                      <RadioGroupItem value="weekly" id="weekly" />
                      <Label htmlFor="weekly">Weekly</Label>
                    </div>
                    <div className="flex items-center space-x-2">
                      <RadioGroupItem value="monthly" id="monthly" />
                      <Label htmlFor="monthly">Monthly</Label>
                    </div>
                  </RadioGroup>
                </div>
                
                {deliveryConfig.schedule?.frequency === 'weekly' && (
                  <div>
                    <Label className="mb-2 block">Day of Week</Label>
                    <RadioGroup 
                      value={String(deliveryConfig.schedule?.dayOfWeek || 1)} 
                      onValueChange={(value) => {
                        const currentSchedule = deliveryConfig.schedule || { 
                          frequency: 'weekly',
                          time: '09:00' 
                        };
                        onConfigChange({
                          ...deliveryConfig,
                          schedule: {
                            ...currentSchedule,
                            dayOfWeek: parseInt(value)
                          }
                        });
                      }}
                      className="grid grid-cols-2 md:grid-cols-4 gap-2"
                    >
                      {['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'].map((day, index) => (
                        <div key={day} className="flex items-center space-x-2">
                          <RadioGroupItem value={String(index + 1)} id={`day-${index + 1}`} />
                          <Label htmlFor={`day-${index + 1}`}>{day}</Label>
                        </div>
                      ))}
                    </RadioGroup>
                  </div>
                )}
                
                {deliveryConfig.schedule?.frequency === 'monthly' && (
                  <div>
                    <Label className="mb-2 block">Day of Month</Label>
                    <div className="flex space-x-2">
                      <Input 
                        type="number" 
                        min="1" 
                        max="31" 
                        value={deliveryConfig.schedule?.dayOfMonth || 1}
                        onChange={(e) => {
                          const value = parseInt(e.target.value);
                          if (value >= 1 && value <= 31) {
                            const currentSchedule = deliveryConfig.schedule || { 
                              frequency: 'monthly', 
                              time: '09:00' 
                            };
                            onConfigChange({
                              ...deliveryConfig,
                              schedule: {
                                ...currentSchedule,
                                dayOfMonth: value
                              }
                            });
                          }
                        }}
                        className="w-20"
                      />
                    </div>
                  </div>
                )}
                
                <div>
                  <Label className="mb-2 block">Time</Label>
                  <Input 
                    type="time" 
                    value={deliveryConfig.schedule?.time || '09:00'}
                    onChange={(e) => {
                      const currentSchedule = deliveryConfig.schedule || { 
                        frequency: deliveryConfig.schedule?.frequency || 'weekly',
                        time: '09:00'
                      };
                      onConfigChange({
                        ...deliveryConfig,
                        schedule: {
                          ...currentSchedule,
                          time: e.target.value
                        }
                      });
                    }}
                    className="w-32"
                  />
                </div>
              </div>
            </TabsContent>
            
            <TabsContent value="triggered" className="space-y-4">
              <p className="text-muted-foreground mb-4">
                Send this survey when specific events occur in your system.
              </p>
              
              <div className="space-y-4">
                <div>
                  <Label className="mb-2 block">Trigger Event</Label>
                  <RadioGroup 
                    value={deliveryConfig.trigger?.type || 'ticket-closed'} 
                    onValueChange={(value) => {
                      const currentTrigger = deliveryConfig.trigger || { 
                        type: 'ticket-closed',
                        delayHours: 24, 
                        sendAutomatically: true 
                      };
                      onConfigChange({
                        ...deliveryConfig,
                        trigger: {
                          ...currentTrigger,
                          type: value as 'ticket-closed' | 'purchase-completed'
                        }
                      });
                    }}
                    className="flex flex-col space-y-2"
                  >
                    <div className="flex items-center space-x-2">
                      <RadioGroupItem value="ticket-closed" id="ticket-closed" />
                      <Label htmlFor="ticket-closed">After ticket is closed</Label>
                    </div>
                    <div className="flex items-center space-x-2">
                      <RadioGroupItem value="purchase-completed" id="purchase-completed" />
                      <Label htmlFor="purchase-completed">After purchase is completed</Label>
                    </div>
                  </RadioGroup>
                </div>
                
                <div>
                  <Label className="mb-2 block">Delay (hours)</Label>
                  <Input 
                    type="number" 
                    min="0" 
                    max="168" 
                    value={deliveryConfig.trigger?.delayHours || 24}
                    onChange={(e) => {
                      const value = parseInt(e.target.value);
                      const currentTrigger = deliveryConfig.trigger || { 
                        type: 'ticket-closed', 
                        delayHours: 24,
                        sendAutomatically: true 
                      };
                      onConfigChange({
                        ...deliveryConfig,
                        trigger: {
                          ...currentTrigger,
                          delayHours: value
                        }
                      });
                    }}
                    className="w-20"
                  />
                </div>
                
                <div className="flex items-center space-x-2">
                  <Switch 
                    id="send-automatically"
                    checked={deliveryConfig.trigger?.sendAutomatically || false}
                    onCheckedChange={(checked) => {
                      const currentTrigger = deliveryConfig.trigger || { 
                        type: 'ticket-closed', 
                        delayHours: 24,
                        sendAutomatically: true
                      };
                      onConfigChange({
                        ...deliveryConfig,
                        trigger: {
                          ...currentTrigger,
                          sendAutomatically: checked
                        }
                      });
                    }}
                  />
                  <Label htmlFor="send-automatically">Send automatically</Label>
                </div>
              </div>
            </TabsContent>
          </Tabs>
        </div>
        
        <div className="border-t pt-6">
          <h3 className="text-lg font-medium mb-3">Email Recipients</h3>
          
          <div className="space-y-4">
            <div className="flex gap-2">
              <div className="flex-1 flex gap-2">
                <Input
                  placeholder="Enter email address"
                  value={emailInput}
                  onChange={(e) => setEmailInput(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') {
                      e.preventDefault();
                      handleAddEmail();
                    }
                  }}
                />
                <Button 
                  variant="outline" 
                  onClick={handleAddEmail}
                >
                  Add
                </Button>
              </div>
              
              <Popover open={commandOpen} onOpenChange={setCommandOpen}>
                <PopoverTrigger asChild>
                  <Button variant="outline" className="w-[220px]">
                    <Search className="mr-2 h-4 w-4" />
                    <span>Search Customers</span>
                    <ChevronsUpDown className="ml-auto h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-[220px] p-0">
                  <Command>
                    <CommandInput placeholder="Search by email..." className="h-9" />
                    <CommandEmpty>No customer found.</CommandEmpty>
                    <CommandGroup>
                      <CommandList>
                        {Array.isArray(customerEmails) && customerEmails.length > 0 ? customerEmails.map((email) => (
                          <CommandItem
                            key={email}
                            value={email}
                            onSelect={handleSelectCustomerEmail}
                          >
                            <Check
                              className={cn(
                                "mr-2 h-4 w-4",
                                (deliveryConfig.emailAddresses || []).includes(email) ? "opacity-100" : "opacity-0"
                              )}
                            />
                            {email}
                          </CommandItem>
                        )) : (
                          <CommandItem disabled>No customers available</CommandItem>
                        )}
                      </CommandList>
                    </CommandGroup>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>
            
            {(!isValidEmail(emailInput) && emailInput.trim() !== '') && (
              <p className="text-sm text-destructive">Please enter a valid email address</p>
            )}
            
            {/* Botones para seleccionar/deseleccionar todos los correos */}
            <div className="flex gap-2">
              <Button 
                variant="outline" 
                size="sm"
                onClick={handleSelectAllEmails}
                className="flex items-center gap-1"
              >
                <CheckCircle className="h-4 w-4" />
                Select All
              </Button>
              <Button 
                variant="outline" 
                size="sm"
                onClick={handleDeselectAllEmails}
                className="flex items-center gap-1"
              >
                <XCircle className="h-4 w-4" />
                Deselect All
              </Button>
            </div>
            
            {(deliveryConfig.emailAddresses && deliveryConfig.emailAddresses.length > 0) ? (
              <div className="border rounded-md p-4">
                <div className="flex flex-wrap gap-2">
                  {(deliveryConfig.emailAddresses || []).map((email) => (
                    <Badge key={email} variant="secondary" className="flex items-center gap-1">
                      {email}
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-4 w-4 rounded-full"
                        onClick={() => handleRemoveEmail(email)}
                      >
                        <X className="h-3 w-3" />
                      </Button>
                    </Badge>
                  ))}
                </div>
              </div>
            ) : (
              <p className="text-sm text-muted-foreground">No email addresses added yet.</p>
            )}
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
