
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

    // Configuración SMTP para Gmail
    const smtpServer = "smtp.gmail.com";
    const smtpPort = 587;
    const smtpUsername = "crisant231@gmail.com";
    const smtpPassword = "avufrkhruqddomsh";
    const senderEmail = "crisant231@gmail.com";
    const senderName = "Sistema de Encuestas";
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

    try {
      // Intentar nuevo método de conexión
      const client = new SmtpClient();
      
      console.log("Connecting to SMTP server...");
      
      // En lugar de usar connectTLS directamente, usar connect primero
      await client.connect({
        hostname: smtpServer,
        port: smtpPort
      });
      
      console.log("Connected to SMTP server, attempting login...");
      
      // Iniciar sesión en el servidor SMTP
      await client.login({
        username: smtpUsername,
        password: smtpPassword
      });
      
      console.log("Logged in to SMTP server successfully");

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
            subject: `Encuesta: ${survey.title}`,
            content: `
<!DOCTYPE html>
<html>
<head>
  <style>
    body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
    .container { max-width: 600px; margin: 0 auto; padding: 20px; }
    .header { background-color: #4a56e2; color: white; padding: 15px; border-radius: 5px; margin-bottom: 20px; }
    .button { display: inline-block; background-color: #4a56e2; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }
    .footer { margin-top: 30px; font-size: 12px; color: #666; }
  </style>
</head>
<body>
  <div class="container">
    <div class="header">
      <h2>Te han invitado a responder una encuesta</h2>
    </div>
    
    <p>Hola,</p>
    <p>Has sido invitado a participar en la encuesta: <strong>${survey.title}</strong></p>
    
    ${survey.description ? `<p>${survey.description}</p>` : ''}
    
    <p>Tu opinión es muy importante para nosotros. Por favor haz clic en el botón de abajo para comenzar la encuesta:</p>
    
    <p><a href="${surveyUrl}" class="button">Responder Encuesta</a></p>
    
    <p>O copia y pega este enlace en tu navegador: ${surveyUrl}</p>
    
    <div class="footer">
      <p>Si recibiste este correo por error, por favor ignóralo.</p>
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
      try {
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
      } catch (logError) {
        // Just log the error but don't fail the request if logging fails
        console.error("Failed to log email sending:", logError);
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
      
      // Try to log the failed email attempt
      try {
        await supabase
          .from("survey_email_logs")
          .insert({
            survey_id: surveyId,
            recipients: emailAddresses,
            status: "failed",
            error_message: smtpError.message,
          });
      } catch (logError) {
        console.error("Failed to log failed email attempt:", logError);
      }
      
      return new Response(
        JSON.stringify({ 
          success: false, 
          error: `SMTP Error: ${smtpError.message}`,
          details: "Es posible que Gmail esté bloqueando el acceso. Verifica que tienes configurado el 'Acceso a aplicaciones menos seguras' o prueba usando una contraseña de aplicación específica para esta app."
        }),
        {
          headers: { ...corsHeaders, "Content-Type": "application/json" },
          status: 500,
        }
      );
    }
  } catch (error) {
    console.error("Error processing request:", error);
    
    return new Response(
      JSON.stringify({ 
        success: false, 
        error: error.message,
        stack: error.stack 
      }),
      {
        headers: { ...corsHeaders, "Content-Type": "application/json" },
        status: 500,
      }
    );
  }
});
