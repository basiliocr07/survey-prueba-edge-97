
import React, { useState } from 'react';
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
              <svg 
                className={cn(
                  "w-10 h-10",
                  (hoverRating !== null && rating <= hoverRating) || 
                  (hoverRating === null && selectedRating !== null && rating <= selectedRating)
                    ? "text-yellow-400 fill-yellow-400" 
                    : "text-gray-300"
                )}
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              <span className="sr-only">{rating} {rating === 1 ? 'estrella' : 'estrellas'}</span>
            </label>
          </div>
        ))}
      </div>
    </div>
  );
}
