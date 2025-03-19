
import React from 'react';
import { Star } from 'lucide-react';
import { cn } from '@/lib/utils';

interface StarRatingProps {
  name: string;
  value: string;
  onChange: (value: string) => void;
  required?: boolean;
}

export default function StarRating({ name, value, onChange, required = false }: StarRatingProps) {
  const selectedValue = value ? parseInt(value) : null;

  return (
    <div className="star-rating-container">
      <div className="star-labels">
        <div>Extremely Dissatisfied</div>
        <div>Dissatisfied</div>
        <div>Neutral</div>
        <div>Satisfied</div>
        <div>Extremely Satisfied</div>
      </div>
      <div className="star-rating">
        {[1, 2, 3, 4, 5].map((rating) => (
          <div key={rating}>
            <input
              type="radio"
              name={name}
              id={`${name}-star${rating}`}
              value={rating.toString()}
              className="sr-only"
              checked={selectedValue === rating}
              onChange={() => onChange(rating.toString())}
              required={required && rating === 1}
            />
            <label
              htmlFor={`${name}-star${rating}`}
              className="cursor-pointer flex items-center justify-center"
            >
              <Star
                className={cn(
                  "h-10 w-10 transition-all",
                  selectedValue >= rating
                    ? "text-yellow-400 fill-yellow-400"
                    : "text-gray-300"
                )}
              />
              <span className="sr-only">{rating} stars</span>
            </label>
          </div>
        ))}
      </div>
    </div>
  );
}
