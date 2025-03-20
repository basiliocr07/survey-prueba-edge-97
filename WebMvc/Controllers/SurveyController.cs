
// Método para enviar encuestas por email a un destinatario específico
[HttpPost]
public async Task<IActionResult> SendSurveyEmail(Guid id, string email)
{
    if (string.IsNullOrEmpty(email))
    {
        TempData["ErrorMessage"] = "El email del destinatario es requerido.";
        return RedirectToAction("Details", new { id });
    }

    try
    {
        var surveyLink = $"{Request.Scheme}://{Request.Host}/survey/{id}";
        var survey = await _surveyService.GetSurveyByIdAsync(id);
        
        await _surveyService.SendSurveyEmailAsync(email, survey.Title, surveyLink);
        
        TempData["SuccessMessage"] = "La encuesta ha sido enviada correctamente al email proporcionado.";
        return RedirectToAction("Details", new { id });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al enviar la encuesta por email: {Message}", ex.Message);
        TempData["ErrorMessage"] = "Ocurrió un error al enviar la encuesta por email.";
        return RedirectToAction("Details", new { id });
    }
}

// Método para enviar encuestas a todos los destinatarios configurados
[HttpPost]
public async Task<IActionResult> SendSurveyToAllRecipients(Guid id)
{
    try
    {
        await _surveyService.SendSurveyEmailsAsync(id);
        
        TempData["SuccessMessage"] = "La encuesta ha sido enviada a todos los destinatarios configurados.";
        return RedirectToAction("Details", new { id });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al enviar la encuesta a todos los destinatarios: {Message}", ex.Message);
        TempData["ErrorMessage"] = "Ocurrió un error al enviar la encuesta a los destinatarios.";
        return RedirectToAction("Details", new { id });
    }
}

// Método para configurar el envío automático
[HttpPost]
public async Task<IActionResult> ConfigureAutomaticDelivery(Guid id, DeliveryConfigViewModel deliveryConfig)
{
    try
    {
        var survey = await _surveyService.GetSurveyByIdAsync(id);
        var updateDto = new UpdateSurveyDto
        {
            Title = survey.Title,
            Description = survey.Description,
            Questions = survey.Questions.Select(q => new CreateQuestionDto
            {
                Title = q.Title,
                Description = q.Description,
                Type = q.Type,
                Required = q.Required,
                Options = q.Options,
                Settings = q.Settings
            }).ToList(),
            DeliveryConfig = new DeliveryConfigDto
            {
                Type = deliveryConfig.Type,
                EmailAddresses = deliveryConfig.EmailAddresses,
                Schedule = new ScheduleDto
                {
                    Frequency = deliveryConfig.Schedule.Frequency,
                    DayOfMonth = deliveryConfig.Schedule.DayOfMonth,
                    DayOfWeek = deliveryConfig.Schedule.DayOfWeek,
                    Time = deliveryConfig.Schedule.Time,
                    StartDate = deliveryConfig.Schedule.StartDate
                },
                Trigger = new TriggerDto
                {
                    Type = deliveryConfig.Trigger.Type,
                    DelayHours = deliveryConfig.Trigger.DelayHours,
                    SendAutomatically = deliveryConfig.Trigger.SendAutomatically
                }
            }
        };
        
        await _surveyService.UpdateSurveyAsync(id, updateDto);
        
        TempData["SuccessMessage"] = "La configuración de envío automático ha sido actualizada.";
        return RedirectToAction("Details", new { id });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al configurar el envío automático: {Message}", ex.Message);
        TempData["ErrorMessage"] = "Ocurrió un error al configurar el envío automático.";
        return RedirectToAction("Details", new { id });
    }
}
