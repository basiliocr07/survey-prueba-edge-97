
import React, { useState } from 'react';
import { Star } from 'lucide-react';
import { cn } from '@/lib/utils';

interface StarRatingProps {
  name: string;
  value?: string;
  onChange: (value: string) => void;
  required?: boolean;
}

const ratingLabels = [
  "Extremely Dissatisfied",
  "Dissatisfied", 
  "Neutral", 
  "Satisfied", 
  "Extremely Satisfied"
];

export default function StarRating({ name, value, onChange, required = false }: StarRatingProps) {
  const [hoverRating, setHoverRating] = useState<number | null>(null);
  const selectedRating = value ? parseInt(value) : null;

  return (
    <div className="space-y-3">
      <div className="star-labels grid grid-cols-5 text-xs text-muted-foreground mb-1">
        {ratingLabels.map((label, i) => (
          <div key={i} className="text-center">
            {label}
          </div>
        ))}
      </div>
      
      <div className="star-rating flex justify-between">
        {[1, 2, 3, 4, 5].map((rating) => (
          <div key={rating} className="text-center">
            <input 
              type="radio" 
              name={name} 
              id={`${name}-star${rating}`} 
              value={rating.toString()} 
              className="sr-only"
              checked={selectedRating === rating}
              onChange={() => onChange(rating.toString())}
              required={required && rating === 1}
            />
            <label 
              htmlFor={`${name}-star${rating}`} 
              className="cursor-pointer block"
              onMouseEnter={() => setHoverRating(rating)}
              onMouseLeave={() => setHoverRating(null)}
            >
              <Star 
                className={cn(
                  "w-10 h-10 transition-colors", 
                  (hoverRating !== null && rating <= hoverRating) || 
                  (hoverRating === null && selectedRating !== null && rating <= selectedRating)
                    ? "text-yellow-400 fill-yellow-400" 
                    : "text-gray-300"
                )}
              />
              <span className="sr-only">{rating} {rating === 1 ? 'estrella' : 'estrellas'}</span>
            </label>
          </div>
        ))}
      </div>
    </div>
  );
}
