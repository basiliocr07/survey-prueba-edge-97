import { useState } from 'react';
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Switch } from "@/components/ui/switch";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Card, CardContent } from "@/components/ui/card";
import { Question, QuestionType } from "@/utils/sampleData";
import { ChevronDown, ChevronUp, Trash2, Plus, GripVertical, Star } from "lucide-react";
import QuestionTypes from "./QuestionTypes";
import { cn } from "@/lib/utils";
import StarRating from './StarRating';
import NPSRating from './NPSRating';

interface QuestionBuilderProps {
  question: Question;
  onChange: (updatedQuestion: Question) => void;
  onDelete: () => void;
  onMoveUp?: () => void;
  onMoveDown?: () => void;
  isFirst?: boolean;
  isLast?: boolean;
}

export default function QuestionBuilder({
  question,
  onChange,
  onDelete,
  onMoveUp,
  onMoveDown,
  isFirst = false,
  isLast = false
}: QuestionBuilderProps) {
  const [isExpanded, setIsExpanded] = useState(true);
  const [showQuestionTypes, setShowQuestionTypes] = useState(false);

  const handleInputChange = (field: keyof Question, value: any) => {
    onChange({ ...question, [field]: value });
  };

  const handleOptionChange = (index: number, value: string) => {
    if (!question.options) return;
    const newOptions = [...question.options];
    newOptions[index] = value;
    handleInputChange('options', newOptions);
  };

  const addOption = () => {
    if (!question.options) {
      handleInputChange('options', ['New option']);
    } else {
      handleInputChange('options', [...question.options, 'New option']);
    }
  };

  const removeOption = (index: number) => {
    if (!question.options) return;
    const newOptions = question.options.filter((_, i) => i !== index);
    handleInputChange('options', newOptions);
  };

  const handleTypeChange = (type: QuestionType) => {
    onChange({
      ...question,
      type,
      options: ['multiple-choice', 'single-choice', 'dropdown', 'matrix', 'ranking'].includes(type)
        ? question.options || ['Option 1', 'Option 2']
        : undefined,
      settings: type === 'rating' 
        ? { min: 1, max: 5 } 
        : type === 'nps'
        ? { min: 0, max: 10 }
        : question.settings
    });
    setShowQuestionTypes(false);
  };

  const renderQuestionOptions = () => {
    if (!['multiple-choice', 'single-choice', 'dropdown', 'ranking'].includes(question.type) || !question.options) {
      return null;
    }

    return (
      <div className="space-y-3 mt-4">
        <Label>Options</Label>
        {question.options.map((option, index) => (
          <div key={index} className="flex items-center gap-2">
            <div className="flex-1">
              <Input
                value={option}
                onChange={(e) => handleOptionChange(index, e.target.value)}
                placeholder={`Option ${index + 1}`}
              />
            </div>
            <Button
              variant="ghost"
              size="icon"
              className="h-9 w-9 text-muted-foreground hover:text-destructive"
              onClick={() => removeOption(index)}
              disabled={question.options && question.options.length <= 2}
            >
              <Trash2 className="h-4 w-4" />
            </Button>
          </div>
        ))}
        <Button
          variant="outline"
          size="sm"
          className="mt-2"
          onClick={addOption}
        >
          <Plus className="h-4 w-4 mr-1" /> Add Option
        </Button>
      </div>
    );
  };

  const renderQuestionPreview = () => {
    if (question.type === 'rating') {
      return (
        <div className="mt-4 border rounded-md p-4 bg-muted/20">
          <Label className="mb-2 block">Preview</Label>
          <StarRating 
            name={`preview-${question.id}`} 
            value="3" 
            onChange={() => {}}
          />
        </div>
      );
    }
    
    if (question.type === 'nps') {
      return (
        <div className="mt-4 border rounded-md p-4 bg-muted/20">
          <Label className="mb-2 block">Preview</Label>
          <NPSRating 
            name={`preview-${question.id}`} 
            value="7" 
            onChange={() => {}}
          />
        </div>
      );
    }
    
    return null;
  };

  return (
    <Card className={cn(
      "w-full transition-all duration-300 mb-4 border",
      isExpanded ? "shadow-sm" : ""
    )}>
      <div className="flex items-center px-4 py-3 border-b bg-muted/30">
        <div className="mr-2 text-muted-foreground">
          <GripVertical className="h-5 w-5 cursor-grab" />
        </div>
        <Button 
          variant="ghost" 
          size="sm" 
          className="p-0 h-auto mr-2"
          onClick={() => setIsExpanded(!isExpanded)}
        >
          {isExpanded ? <ChevronUp className="h-4 w-4" /> : <ChevronDown className="h-4 w-4" />}
        </Button>
        <div className="flex-1 font-medium truncate">
          {question.title || "Untitled Question"}
        </div>
        <div className="flex items-center gap-1">
          {!isFirst && onMoveUp && (
            <Button variant="ghost" size="icon" className="h-8 w-8" onClick={onMoveUp}>
              <ChevronUp className="h-4 w-4" />
            </Button>
          )}
          {!isLast && onMoveDown && (
            <Button variant="ghost" size="icon" className="h-8 w-8" onClick={onMoveDown}>
              <ChevronDown className="h-4 w-4" />
            </Button>
          )}
          <Button 
            variant="ghost" 
            size="icon" 
            className="h-8 w-8 text-muted-foreground hover:text-destructive" 
            onClick={onDelete}
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>
      </div>

      {isExpanded && (
        <CardContent className="p-4 space-y-4 animate-slide-down">
          <div className="space-y-2">
            <div>
              <Label htmlFor={`question-${question.id}-title`}>Question</Label>
              <Input
                id={`question-${question.id}-title`}
                value={question.title}
                onChange={(e) => handleInputChange('title', e.target.value)}
                placeholder="Enter your question"
                className="mt-1"
              />
            </div>
            
            <div>
              <Label htmlFor={`question-${question.id}-description`}>Description (optional)</Label>
              <Textarea
                id={`question-${question.id}-description`}
                value={question.description || ''}
                onChange={(e) => handleInputChange('description', e.target.value)}
                placeholder="Add a description to provide more context"
                className="mt-1 min-h-[80px]"
              />
            </div>
          </div>

          <div>
            <Button
              variant="outline"
              onClick={() => setShowQuestionTypes(!showQuestionTypes)}
              className="w-full justify-between group"
            >
              <span>Question Type: <span className="font-medium">{question.type.replace('-', ' ')}</span></span>
              <ChevronDown className={cn(
                "h-4 w-4 transition-transform", 
                showQuestionTypes ? "rotate-180" : ""
              )} />
            </Button>
            
            {showQuestionTypes && (
              <div className="mt-3 p-3 border rounded-md animate-fade-in">
                <QuestionTypes
                  onSelectType={handleTypeChange}
                  selectedType={question.type}
                />
              </div>
            )}
          </div>

          {renderQuestionOptions()}
          {renderQuestionPreview()}

          <div className="flex items-center space-x-2 pt-2">
            <Switch 
              id={`question-${question.id}-required`}
              checked={question.required}
              onCheckedChange={(checked) => handleInputChange('required', checked)}
            />
            <Label htmlFor={`question-${question.id}-required`}>Required question</Label>
          </div>
        </CardContent>
      )}
    </Card>
  );
}
