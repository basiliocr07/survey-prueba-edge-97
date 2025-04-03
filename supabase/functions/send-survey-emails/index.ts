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
  if (req.method === "OPTIONS") {
    return new Response("ok", { headers: corsHeaders });
  }

  try {
    console.log("Received request to send survey emails");
    
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
      const client = new SmtpClient();
      
      console.log("Connecting to SMTP server...");
      
      await client.connect({
        hostname: smtpServer,
        port: smtpPort
      });
      
      console.log("Connected to SMTP server, attempting login...");
      
      await client.login({
        username: smtpUsername,
        password: smtpPassword
      });
      
      console.log("Logged in to SMTP server successfully");

      const surveyUrl = `${frontendUrl}/take-survey/${surveyId}`;
      console.log("Survey URL:", surveyUrl);

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
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Invitación a Encuesta: ${survey.title}</title>
    <style>
        * {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        body {
            background-color: #f9fafb;
            line-height: 1.5;
        }
        .container {
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }
        .header {
            background-color: #4f46e5;
            color: white;
            padding: 20px;
            text-align: center;
        }
        .header h1 {
            font-size: 24px;
            margin-bottom: 5px;
        }
        .content {
            padding: 30px 20px;
            color: #374151;
        }
        .survey-info {
            background-color: #f3f4f6;
            border-radius: 6px;
            padding: 15px;
            margin: 20px 0;
        }
        .survey-title {
            font-size: 18px;
            font-weight: 600;
            color: #111827;
            margin-bottom: 10px;
        }
        .button-container {
            text-align: center;
            margin: 30px 0;
        }
        .button {
            display: inline-block;
            background-color: #4f46e5;
            color: white;
            text-decoration: none;
            padding: 12px 24px;
            border-radius: 6px;
            font-weight: 600;
            transition: background-color 0.3s;
        }
        .button:hover {
            background-color: #4338ca;
        }
        .survey-link {
            background-color: #f3f4f6;
            border-radius: 6px;
            padding: 12px;
            margin-top: 20px;
            word-break: break-all;
            font-size: 14px;
            text-align: center;
        }
        .footer {
            border-top: 1px solid #e5e7eb;
            padding: 20px;
            text-align: center;
            font-size: 14px;
            color: #6b7280;
        }
        .logo {
            max-width: 120px;
            margin-bottom: 10px;
        }
        @media only screen and (max-width: 550px) {
            .container {
                width: 100%;
                margin: 0;
                border-radius: 0;
            }
            .header h1 {
                font-size: 20px;
            }
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>Invitación a Encuesta</h1>
            <p>Tu opinión es importante para nosotros</p>
        </div>
        
        <div class="content">
            <p>Hola,</p>
            <p>Te invitamos a participar en nuestra encuesta:</p>
            
            <div class="survey-info">
                <div class="survey-title">${survey.title}</div>
                ${survey.description ? `<p>${survey.description}</p>` : ''}
            </div>
            
            <p>Tus respuestas nos ayudarán a mejorar nuestros servicios. La encuesta solo tomará unos minutos de tu tiempo.</p>
            
            <div class="button-container">
                <a href="${surveyUrl}" class="button">Comenzar Encuesta</a>
            </div>
            
            <p>O si prefieres, puedes copiar y pegar este enlace en tu navegador:</p>
            <div class="survey-link">
                ${surveyUrl}
            </div>
        </div>
        
        <div class="footer">
            <p>Si recibiste este correo por error, puedes ignorarlo.</p>
            <p>&copy; ${new Date().getFullYear()} Sistema de Encuestas</p>
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

      await client.close();
      console.log("SMTP connection closed");

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
