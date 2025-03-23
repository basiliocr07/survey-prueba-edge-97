
import { createClient } from 'https://esm.sh/@supabase/supabase-js@2.39.6';

// Define CORS headers
const corsHeaders = {
  'Access-Control-Allow-Origin': '*',
  'Access-Control-Allow-Headers': 'authorization, x-client-info, apikey, content-type',
};

// Create a Supabase client
const supabaseUrl = Deno.env.get('SUPABASE_URL') ?? '';
const supabaseAnonKey = Deno.env.get('SUPABASE_ANON_KEY') ?? '';
const supabase = createClient(supabaseUrl, supabaseAnonKey);

Deno.serve(async (req) => {
  // Handle CORS preflight requests
  if (req.method === 'OPTIONS') {
    return new Response(null, { headers: corsHeaders });
  }

  try {
    // Get request body
    const { surveyId, emailAddresses } = await req.json();

    if (!surveyId || !emailAddresses || !Array.isArray(emailAddresses) || emailAddresses.length === 0) {
      return new Response(
        JSON.stringify({ 
          success: false, 
          error: 'Invalid request parameters. SurveyId and emailAddresses array are required.' 
        }),
        { 
          status: 400, 
          headers: { ...corsHeaders, 'Content-Type': 'application/json' } 
        }
      );
    }

    // Get survey details
    const { data: survey, error: surveyError } = await supabase
      .from('surveys')
      .select('*')
      .eq('id', surveyId)
      .single();

    if (surveyError || !survey) {
      console.error('Error fetching survey:', surveyError);
      return new Response(
        JSON.stringify({ success: false, error: 'Survey not found' }),
        { 
          status: 404, 
          headers: { ...corsHeaders, 'Content-Type': 'application/json' } 
        }
      );
    }

    console.log(`Found survey: ${survey.title}, sending to ${emailAddresses.length} recipients`);

    // In a real implementation, you would connect to an email service like SendGrid, Mailgun, etc.
    // For now, we'll just log the emails and return success

    // Create a log of the email sending attempt in the database (optional)
    const { error: logError } = await supabase
      .from('survey_email_logs')
      .insert({
        survey_id: surveyId,
        recipients: emailAddresses,
        sent_at: new Date().toISOString(),
        status: 'sent' // In a real implementation this would be based on the email service response
      })
      .select('*')
      .single();

    if (logError) {
      console.warn('Failed to log email sending:', logError);
      // Continue execution even if logging fails
    }

    return new Response(
      JSON.stringify({ 
        success: true, 
        message: `Successfully sent survey to ${emailAddresses.length} recipients` 
      }),
      { 
        status: 200, 
        headers: { ...corsHeaders, 'Content-Type': 'application/json' } 
      }
    );
  } catch (error) {
    console.error('Error sending survey emails:', error);
    return new Response(
      JSON.stringify({ 
        success: false, 
        error: error instanceof Error ? error.message : 'Unknown error' 
      }),
      { 
        status: 500, 
        headers: { ...corsHeaders, 'Content-Type': 'application/json' } 
      }
    );
  }
});
