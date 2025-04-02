
import { serve } from "https://deno.land/std@0.168.0/http/server.ts";
import { createClient } from "https://esm.sh/@supabase/supabase-js@2.38.0";

const corsHeaders = {
  "Access-Control-Allow-Origin": "*",
  "Access-Control-Allow-Headers": "authorization, x-client-info, apikey, content-type",
};

interface RequestData {
  surveyId: string;
  config: {
    type: string;
    emailAddresses: string[];
    schedule?: {
      frequency: string;
      dayOfMonth?: number;
      dayOfWeek?: number;
      time?: string;
      startDate?: string;
    };
    trigger?: {
      type: string;
      delayHours: number;
      sendAutomatically: boolean;
    };
  };
}

serve(async (req) => {
  // Handle OPTIONS request for CORS
  if (req.method === "OPTIONS") {
    return new Response("ok", { headers: corsHeaders });
  }

  try {
    console.log("Received request to schedule survey emails");
    
    // Parse the request body
    const requestData = await req.json();
    const { surveyId, config } = requestData as RequestData;

    console.log("Request data:", { surveyId, config });

    if (!surveyId || !config) {
      console.error("Invalid request parameters");
      return new Response(
        JSON.stringify({ success: false, error: "Invalid request parameters" }),
        {
          headers: { ...corsHeaders, "Content-Type": "application/json" },
          status: 400,
        }
      );
    }

    // Create a Supabase client
    const supabaseUrl = Deno.env.get("SUPABASE_URL") || "";
    const supabaseServiceKey = Deno.env.get("SUPABASE_SERVICE_ROLE_KEY") || "";
    
    if (!supabaseUrl || !supabaseServiceKey) {
      console.error("Missing Supabase configuration");
      return new Response(
        JSON.stringify({ success: false, error: "Missing Supabase configuration" }),
        {
          headers: { ...corsHeaders, "Content-Type": "application/json" },
          status: 500,
        }
      );
    }
    
    const supabase = createClient(supabaseUrl, supabaseServiceKey);

    // Get the survey from the database
    const { data: survey, error: surveyError } = await supabase
      .from("surveys")
      .select("*")
      .eq("id", surveyId)
      .single();

    if (surveyError || !survey) {
      console.error("Survey not found:", surveyError);
      return new Response(
        JSON.stringify({ success: false, error: "Survey not found" }),
        {
          headers: { ...corsHeaders, "Content-Type": "application/json" },
          status: 404,
        }
      );
    }

    // Store the schedule in a schedules table
    const { data: scheduleData, error: scheduleError } = await supabase
      .from("email_schedules")
      .upsert({
        survey_id: surveyId,
        config: config,
        status: "active",
        created_at: new Date().toISOString(),
      })
      .select();

    if (scheduleError) {
      console.error("Error saving schedule:", scheduleError);
      return new Response(
        JSON.stringify({ success: false, error: "Failed to save schedule" }),
        {
          headers: { ...corsHeaders, "Content-Type": "application/json" },
          status: 500,
        }
      );
    }

    // Log the scheduling operation
    try {
      await supabase
        .from("survey_email_logs")
        .insert({
          survey_id: surveyId,
          recipients: config.emailAddresses,
          status: "scheduled",
          error_message: null,
        });
    } catch (logError) {
      // Just log the error but don't fail the request if logging fails
      console.error("Failed to log email scheduling:", logError);
    }

    return new Response(
      JSON.stringify({ 
        success: true, 
        message: "Survey email delivery scheduled successfully",
        scheduleId: scheduleData?.[0]?.id
      }),
      {
        headers: { ...corsHeaders, "Content-Type": "application/json" },
        status: 200,
      }
    );
  } catch (error) {
    console.error("Error processing request:", error);
    
    return new Response(
      JSON.stringify({ success: false, error: error.message }),
      {
        headers: { ...corsHeaders, "Content-Type": "application/json" },
        status: 500,
      }
    );
  }
});
