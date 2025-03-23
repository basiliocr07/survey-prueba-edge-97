
import { serve } from "https://deno.land/std@0.168.0/http/server.ts";
import { createClient } from "https://esm.sh/@supabase/supabase-js@2.38.0";
import { SmtpClient } from "https://deno.land/x/smtp@v0.7.0/mod.ts";

interface RequestData {
  surveyId: string;
  emailAddresses: string[];
}

const corsHeaders = {
  "Access-Control-Allow-Origin": "*",
  "Access-Control-Allow-Headers": "authorization, x-client-info, apikey, content-type",
};

serve(async (req) => {
  // Handle OPTIONS request for CORS
  if (req.method === "OPTIONS") {
    return new Response("ok", { headers: corsHeaders });
  }

  try {
    console.log("Received request to send survey emails");
    
    // Parse the request body
    const requestData = await req.json();
    const { surveyId, emailAddresses } = requestData as RequestData;

    console.log("Request data:", { surveyId, emailAddresses });

    if (!surveyId || !emailAddresses || !Array.isArray(emailAddresses) || emailAddresses.length === 0) {
      console.error("Invalid request parameters:", { surveyId, emailAddresses });
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

    // Get SMTP settings from environment variables
    const smtpServer = Deno.env.get("SMTP_SERVER") || "";
    const smtpPort = parseInt(Deno.env.get("SMTP_PORT") || "587", 10);
    const smtpUsername = Deno.env.get("SMTP_USERNAME") || "";
    const smtpPassword = Deno.env.get("SMTP_PASSWORD") || "";
    const senderEmail = Deno.env.get("SENDER_EMAIL") || "";
    const senderName = Deno.env.get("SENDER_NAME") || "Survey System";
    const frontendUrl = Deno.env.get("FRONTEND_URL") || "http://localhost:5173";

    console.log("SMTP Configuration:", { 
      smtpServer, 
      smtpPort, 
      smtpUsername: smtpUsername ? "Set" : "Not set", 
      smtpPassword: smtpPassword ? "Set" : "Not set",
      senderEmail,
      senderName,
      frontendUrl
    });

    if (!smtpServer || !smtpUsername || !smtpPassword || !senderEmail) {
      console.error("SMTP configuration is missing");
      return new Response(
        JSON.stringify({ success: false, error: "SMTP configuration is missing" }),
        {
          headers: { ...corsHeaders, "Content-Type": "application/json" },
          status: 500,
        }
      );
    }

    try {
      // Create an SMTP client
      const client = new SmtpClient();
      
      console.log("Connecting to SMTP server...");
      await client.connectTLS({
        hostname: smtpServer,
        port: smtpPort,
        username: smtpUsername,
        password: smtpPassword,
      });
      console.log("Connected to SMTP server successfully");

      // Generate survey URL
      const surveyUrl = `${frontendUrl}/take-survey/${surveyId}`;
      console.log("Survey URL:", surveyUrl);

      // Send email to each recipient
      const emailResults = [];
      for (const email of emailAddresses) {
        try {
          console.log(`Sending email to ${email}...`);
          await client.send({
            from: `${senderName} <${senderEmail}>`,
            to: email,
            subject: `Survey: ${survey.title}`,
            content: `
<!DOCTYPE html>
<html>
<head>
  <style>
    body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
    .container { max-width: 600px; margin: 0 auto; padding: 20px; }
    .header { background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin-bottom: 20px; }
    .button { display: inline-block; background-color: #4a56e2; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }
    .footer { margin-top: 30px; font-size: 12px; color: #666; }
  </style>
</head>
<body>
  <div class="container">
    <div class="header">
      <h2>You've been invited to take a survey</h2>
    </div>
    
    <p>Hello,</p>
    <p>You've been invited to participate in the survey: <strong>${survey.title}</strong></p>
    
    ${survey.description ? `<p>${survey.description}</p>` : ''}
    
    <p>Your feedback is important to us. Please click the button below to start the survey:</p>
    
    <p><a href="${surveyUrl}" class="button">Take Survey</a></p>
    
    <p>Or copy and paste this URL into your browser: ${surveyUrl}</p>
    
    <div class="footer">
      <p>If you received this email by mistake, please disregard it.</p>
    </div>
  </div>
</body>
</html>
          `,
            html: true,
          });
          console.log(`Email sent to ${email} successfully`);
          emailResults.push({ email, success: true });
        } catch (emailError) {
          console.error(`Failed to send email to ${email}:`, emailError);
          emailResults.push({ email, success: false, error: emailError.message });
        }
      }

      // Close the SMTP connection
      await client.close();
      console.log("SMTP connection closed");

      // Log the email sending in the database
      const { error: logError } = await supabase
        .from("survey_email_logs")
        .insert({
          survey_id: surveyId,
          recipients: emailAddresses,
          status: "sent",
          error_message: null,
        });

      if (logError) {
        console.error("Failed to log email sending:", logError);
      } else {
        console.log("Email sending logged successfully");
      }

      return new Response(
        JSON.stringify({ 
          success: true, 
          message: `Emails sent to ${emailResults.filter(r => r.success).length} recipients`,
          results: emailResults
        }),
        {
          headers: { ...corsHeaders, "Content-Type": "application/json" },
          status: 200,
        }
      );
    } catch (smtpError) {
      console.error("SMTP Error:", smtpError);
      return new Response(
        JSON.stringify({ success: false, error: `SMTP Error: ${smtpError.message}` }),
        {
          headers: { ...corsHeaders, "Content-Type": "application/json" },
          status: 500,
        }
      );
    }
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
