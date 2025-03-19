
import React from 'react';
import { cn } from '@/lib/utils';

interface NPSRatingProps {
  name: string;
  value?: string;
  onChange: (value: string) => void;
  required?: boolean;
}

export default function NPSRating({ name, value, onChange, required = false }: NPSRatingProps) {
  const selectedValue = value ? parseInt(value) : null;

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-11 gap-1 w-full">
        {[0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10].map((rating) => (
          <div key={rating} className="text-center">
            <input
              type="radio"
              name={name}
              id={`${name}-nps${rating}`}
              value={rating.toString()}
              className="sr-only"
              checked={selectedValue === rating}
              onChange={() => onChange(rating.toString())}
              required={required && rating === 0}
            />
            <label
              htmlFor={`${name}-nps${rating}`}
              className={cn(
                "flex items-center justify-center h-10 rounded-md cursor-pointer border transition-all",
                selectedValue === rating
                  ? "bg-primary text-primary-foreground border-primary"
                  : "bg-background hover:bg-accent border-input"
              )}
            >
              {rating}
            </label>
          </div>
        ))}
      </div>
      
      <div className="flex justify-between text-sm text-muted-foreground px-1">
        <div>No es probable</div>
        <div>Neutral</div>
        <div>Muy probable</div>
      </div>
    </div>
  );
}
