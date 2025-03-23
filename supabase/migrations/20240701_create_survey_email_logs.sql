
-- Create a table to store survey email sending logs
CREATE TABLE IF NOT EXISTS public.survey_email_logs (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  survey_id UUID NOT NULL REFERENCES public.surveys(id) ON DELETE CASCADE,
  recipients JSONB NOT NULL, -- Array of email addresses
  sent_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now(),
  status TEXT NOT NULL, -- 'sent', 'failed', etc.
  error_message TEXT,
  created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now()
);

-- Add index for faster lookups
CREATE INDEX IF NOT EXISTS survey_email_logs_survey_id_idx ON public.survey_email_logs(survey_id);
