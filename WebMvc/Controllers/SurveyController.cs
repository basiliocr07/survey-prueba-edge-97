
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
