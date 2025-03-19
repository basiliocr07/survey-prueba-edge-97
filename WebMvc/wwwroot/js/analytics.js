
document.addEventListener('DOMContentLoaded', function() {
    // Initialize any charts that need to be created on page load
    initializeCharts();
    
    // Add event listeners for filter controls
    setupFilterListeners();
    
    // Initialize any animations
    setupAnimations();
    
    // Initialize export functionality
    setupExportOptions();
    
    // Setup response details functionality
    setupResponseDetailsHandlers();
    
    // Initialize filter controls
    initializeFilterControls();
});

function initializeCharts() {
    // Check if Chart.js is loaded
    if (typeof Chart === 'undefined') {
        console.warn('Chart.js is not loaded. Charts will not be rendered.');
        return;
    }
    
    // Response trends chart
    if (document.getElementById('responseTrendsChart')) {
        renderResponseTrendsChart();
    }
    
    // Demographics chart
    if (document.getElementById('demographicsChart')) {
        renderDemographicsChart();
    }
    
    // Device distribution chart
    if (document.getElementById('deviceChart')) {
        renderDeviceDistributionChart();
    }
    
    // Question type distribution chart
    if (document.getElementById('questionTypeChart')) {
        renderQuestionTypeChart();
    }
    
    // Rating distribution chart
    if (document.getElementById('ratingDistributionChart')) {
        renderRatingDistributionChart();
    }
    
    // Completion time chart
    if (document.getElementById('completionTimeChart')) {
        renderCompletionTimeChart();
    }
    
    // Time of day response chart
    if (document.getElementById('timeOfDayChart')) {
        renderTimeOfDayChart();
    }
}

function renderResponseTrendsChart() {
    const ctx = document.getElementById('responseTrendsChart').getContext('2d');
    
    // Get data from the data attribute or use placeholder data
    let labels = [];
    let data = [];
    
    try {
        const chartData = JSON.parse(document.getElementById('responseTrendsChart').dataset.chart);
        labels = chartData.labels || [];
        data = chartData.data || [];
    } catch (error) {
        console.warn('Error parsing chart data:', error);
        labels = ['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb', 'Dom'];
        data = [5, 8, 12, 7, 9, 3, 2];
    }
    
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Respuestas',
                data: data,
                fill: true,
                backgroundColor: 'rgba(59, 130, 246, 0.1)',
                borderColor: '#3b82f6',
                tension: 0.4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0
                    }
                }
            }
        }
    });
}

function renderDemographicsChart() {
    const ctx = document.getElementById('demographicsChart').getContext('2d');
    
    // Get data from the data attribute or use placeholder data
    let labels = [];
    let data = [];
    
    try {
        const chartData = JSON.parse(document.getElementById('demographicsChart').dataset.chart);
        labels = chartData.labels || [];
        data = chartData.data || [];
    } catch (error) {
        console.warn('Error parsing demographics data:', error);
        labels = ['Empresas', 'Educación', 'Gobierno', 'Individuos'];
        data = [40, 25, 15, 20];
    }
    
    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: [
                    '#3b82f6',
                    '#10b981',
                    '#f59e0b',
                    '#ef4444'
                ]
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right'
                }
            }
        }
    });
}

function renderDeviceDistributionChart() {
    const ctx = document.getElementById('deviceChart').getContext('2d');
    
    // Get data from the data attribute or use placeholder data
    let labels = [];
    let data = [];
    
    try {
        const chartData = JSON.parse(document.getElementById('deviceChart').dataset.chart);
        labels = chartData.labels || [];
        data = chartData.data || [];
    } catch (error) {
        console.warn('Error parsing device data:', error);
        labels = ['Desktop', 'Mobile', 'Tablet', 'Other'];
        data = [45, 40, 10, 5];
    }
    
    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: [
                    '#3b82f6',
                    '#10b981',
                    '#f59e0b',
                    '#ef4444'
                ]
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right'
                }
            }
        }
    });
}

function renderQuestionTypeChart() {
    const ctx = document.getElementById('questionTypeChart').getContext('2d');
    
    // Get data from the data attribute or use placeholder data
    let labels = [];
    let data = [];
    
    try {
        const chartData = JSON.parse(document.getElementById('questionTypeChart').dataset.chart);
        labels = chartData.labels || [];
        data = chartData.data || [];
    } catch (error) {
        console.warn('Error parsing question type data:', error);
        labels = ['Text', 'Multiple Choice', 'Single Choice', 'Rating'];
        data = [30, 40, 20, 10];
    }
    
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Distribution',
                data: data,
                backgroundColor: '#3b82f6'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0
                    }
                }
            }
        }
    });
}

function renderRatingDistributionChart() {
    const ctx = document.getElementById('ratingDistributionChart').getContext('2d');
    
    // Get data from the data attribute or use placeholder data
    let labels = [];
    let data = [];
    
    try {
        const chartData = JSON.parse(document.getElementById('ratingDistributionChart').dataset.chart);
        labels = chartData.labels || [];
        data = chartData.data || [];
    } catch (error) {
        console.warn('Error parsing rating distribution data:', error);
        labels = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10'];
        data = [2, 3, 4, 6, 8, 12, 18, 22, 15, 10];
    }
    
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Ratings',
                data: data,
                backgroundColor: '#10b981'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0
                    }
                }
            }
        }
    });
}

function renderCompletionTimeChart() {
    const ctx = document.getElementById('completionTimeChart').getContext('2d');
    
    // Get data from the data attribute or use placeholder data
    let labels = [];
    let data = [];
    
    try {
        const chartData = JSON.parse(document.getElementById('completionTimeChart').dataset.chart);
        labels = chartData.labels || [];
        data = chartData.data || [];
    } catch (error) {
        console.warn('Error parsing completion time data:', error);
        labels = ['< 1 min', '1-2 min', '2-5 min', '5-10 min', '> 10 min'];
        data = [15, 25, 35, 20, 5];
    }
    
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Completion Time',
                data: data,
                backgroundColor: '#f59e0b'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0
                    }
                }
            }
        }
    });
}

function renderTimeOfDayChart() {
    const ctx = document.getElementById('timeOfDayChart').getContext('2d');
    
    // Get data from the data attribute or use placeholder data
    let labels = [];
    let data = [];
    
    try {
        const chartData = JSON.parse(document.getElementById('timeOfDayChart').dataset.chart);
        labels = chartData.labels || [];
        data = chartData.data || [];
    } catch (error) {
        console.warn('Error parsing time of day data:', error);
        labels = ['Mañana (6-12)', 'Tarde (12-18)', 'Noche (18-24)', 'Madrugada (0-6)'];
        data = [25, 40, 30, 5];
    }
    
    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: [
                    '#f59e0b',
                    '#3b82f6',
                    '#8b5cf6',
                    '#64748b'
                ]
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right'
                }
            }
        }
    });
}

function setupFilterListeners() {
    // Add event listeners for date range picker, survey selector, etc.
    const dateRangeSelector = document.querySelector('.date-range-selector');
    if (dateRangeSelector) {
        dateRangeSelector.addEventListener('change', function() {
            // Reload data based on selected date range
            console.log('Date range changed:', this.value);
            applyFilters();
        });
    }
    
    const surveySelector = document.querySelector('.survey-selector');
    if (surveySelector) {
        surveySelector.addEventListener('change', function() {
            // Reload data based on selected survey
            console.log('Survey selection changed:', this.value);
            applyFilters();
        });
    }
    
    // Más filtros (como en la versión React)
    const deviceFilter = document.querySelector('.device-filter');
    if (deviceFilter) {
        deviceFilter.addEventListener('change', function() {
            console.log('Device filter changed:', this.value);
            applyFilters();
        });
    }
    
    const statusFilter = document.querySelector('.status-filter');
    if (statusFilter) {
        statusFilter.addEventListener('change', function() {
            console.log('Status filter changed:', this.value);
            applyFilters();
        });
    }
    
    // Botón de aplicar filtros
    const applyFilterBtn = document.querySelector('.apply-filter-btn');
    if (applyFilterBtn) {
        applyFilterBtn.addEventListener('click', function() {
            applyFilters();
        });
    }
    
    // Botón de resetear filtros
    const resetFilterBtn = document.querySelector('.reset-filter-btn');
    if (resetFilterBtn) {
        resetFilterBtn.addEventListener('click', function() {
            resetFilters();
        });
    }
}

function applyFilters() {
    // Mostrar spinner o indicador de carga
    const loadingIndicator = document.querySelector('.loading-indicator');
    if (loadingIndicator) {
        loadingIndicator.classList.remove('hidden');
    }
    
    // Recopilar todos los valores de filtros
    const filters = {
        surveyId: document.querySelector('.survey-selector')?.value,
        dateRange: document.querySelector('.date-range-selector')?.value,
        device: document.querySelector('.device-filter')?.value,
        status: document.querySelector('.status-filter')?.value
    };
    
    // Construir URL con parámetros
    let url = window.location.pathname + '?';
    for (const [key, value] of Object.entries(filters)) {
        if (value) {
            url += `${key}=${encodeURIComponent(value)}&`;
        }
    }
    url = url.slice(0, -1); // Eliminar el último &
    
    // Redirigir a la nueva URL con filtros
    window.location.href = url;
}

function resetFilters() {
    // Redirigir a la misma página sin filtros
    window.location.href = window.location.pathname;
}

function setupAnimations() {
    // Add animation classes to elements as needed
    const cards = document.querySelectorAll('.card');
    cards.forEach((card, index) => {
        // Add staggered animation delay
        card.style.animationDelay = `${index * 0.1}s`;
        card.classList.add('animate-fade-in');
    });
    
    // Animación para tabla de respuestas
    const tableRows = document.querySelectorAll('tbody tr');
    tableRows.forEach((row, index) => {
        row.style.animationDelay = `${index * 0.05}s`;
        row.classList.add('animate-fade-in');
    });
    
    // Animación para gráficos
    const charts = document.querySelectorAll('.chart-container');
    charts.forEach((chart, index) => {
        chart.style.animationDelay = `${index * 0.2}s`;
        chart.classList.add('animate-scale-in');
    });
}

function setupExportOptions() {
    // Exportar a PDF
    const exportPdfBtn = document.querySelector('.export-pdf-btn');
    if (exportPdfBtn) {
        exportPdfBtn.addEventListener('click', function() {
            exportToPDF();
        });
    }
    
    // Exportar a CSV
    const exportCsvBtn = document.querySelector('.export-csv-btn');
    if (exportCsvBtn) {
        exportCsvBtn.addEventListener('click', function() {
            exportToCSV();
        });
    }
    
    // Exportar a Excel
    const exportExcelBtn = document.querySelector('.export-excel-btn');
    if (exportExcelBtn) {
        exportExcelBtn.addEventListener('click', function() {
            exportToExcel();
        });
    }
}

function exportToPDF() {
    // Mostrar notificación de proceso
    showToast('Generando PDF...', 'info');
    
    // Recolectar los parámetros actuales
    const surveyId = document.querySelector('.survey-selector')?.value;
    const dateRange = document.querySelector('.date-range-selector')?.value;
    
    // Construir URL para la API de exportación
    const exportUrl = `/Analytics/ExportPDF?surveyId=${surveyId || ''}&dateRange=${dateRange || ''}`;
    
    // Redirigir a la URL de exportación
    window.location.href = exportUrl;
}

function exportToCSV() {
    // Mostrar notificación de proceso
    showToast('Generando CSV...', 'info');
    
    // Recolectar los parámetros actuales
    const surveyId = document.querySelector('.survey-selector')?.value;
    const dateRange = document.querySelector('.date-range-selector')?.value;
    
    // Construir URL para la API de exportación
    const exportUrl = `/Analytics/ExportCSV?surveyId=${surveyId || ''}&dateRange=${dateRange || ''}`;
    
    // Redirigir a la URL de exportación
    window.location.href = exportUrl;
}

function exportToExcel() {
    // Mostrar notificación de proceso
    showToast('Generando Excel...', 'info');
    
    // Recolectar los parámetros actuales
    const surveyId = document.querySelector('.survey-selector')?.value;
    const dateRange = document.querySelector('.date-range-selector')?.value;
    
    // Construir URL para la API de exportación
    const exportUrl = `/Analytics/ExportExcel?surveyId=${surveyId || ''}&dateRange=${dateRange || ''}`;
    
    // Redirigir a la URL de exportación
    window.location.href = exportUrl;
}

function setupResponseDetailsHandlers() {
    // Manejadores para ver detalles de respuestas
    const viewDetailsBtns = document.querySelectorAll('.view-details-btn');
    viewDetailsBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            const responseId = this.dataset.responseId;
            if (responseId) {
                window.location.href = `/Analytics/ResponseDetail?id=${responseId}`;
            }
        });
    });
    
    // Manejador para expandir/colapsar secciones de respuestas
    const toggleSections = document.querySelectorAll('.toggle-section');
    toggleSections.forEach(toggle => {
        toggle.addEventListener('click', function() {
            const targetId = this.dataset.target;
            const targetSection = document.getElementById(targetId);
            if (targetSection) {
                targetSection.classList.toggle('hidden');
                // Cambiar icono
                const icon = this.querySelector('i');
                if (icon) {
                    icon.classList.toggle('fa-chevron-down');
                    icon.classList.toggle('fa-chevron-up');
                }
            }
        });
    });
}

function initializeFilterControls() {
    // Datepicker para filtro de fechas (utilizando flatpickr si está disponible)
    const dateRangePicker = document.getElementById('dateRangePicker');
    if (dateRangePicker && typeof flatpickr !== 'undefined') {
        flatpickr(dateRangePicker, {
            mode: "range",
            dateFormat: "Y-m-d",
            onChange: function(selectedDates, dateStr, instance) {
                console.log('Selected date range:', dateStr);
            }
        });
    }
    
    // Inicializar selectores avanzados (usando select2 si está disponible)
    const enhancedSelects = document.querySelectorAll('.enhanced-select');
    if (enhancedSelects.length > 0 && typeof $ !== 'undefined' && typeof $.fn.select2 !== 'undefined') {
        $(enhancedSelects).select2({
            placeholder: "Select an option",
            allowClear: true
        });
    }
}

function refreshAnalytics() {
    // This function would make an AJAX call to refresh analytics data
    console.log('Refreshing analytics data...');
    
    // Show loading indicators
    const charts = document.querySelectorAll('.chart-container');
    charts.forEach(chart => {
        chart.innerHTML = '<div class="loading-spinner"></div>';
    });
    
    // Show toast notification
    showToast('Actualizando datos de análisis...', 'info');
    
    // Make AJAX call to refresh data
    fetch('/Analytics/RefreshAnalytics', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            // Si necesitas CSRF token lo agregarías aquí
            'X-CSRF-TOKEN': document.querySelector('input[name="__RequestVerificationToken"]')?.value
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        showToast('Datos actualizados correctamente', 'success');
        // Reload the page to show refreshed data
        window.location.reload();
    })
    .catch(error => {
        console.error('Error refreshing analytics:', error);
        showToast('Error al actualizar los datos', 'error');
        
        // Restore charts from error state
        initializeCharts();
    });
}

function showToast(message, type = 'info') {
    // Crear elemento toast si no existe
    let toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.className = 'fixed bottom-4 right-4 z-50 flex flex-col gap-2';
        document.body.appendChild(toastContainer);
    }
    
    // Crear el toast
    const toast = document.createElement('div');
    toast.className = `p-4 rounded shadow-md animate-fade-in flex items-center ${
        type === 'success' ? 'bg-green-500 text-white' :
        type === 'error' ? 'bg-red-500 text-white' :
        type === 'warning' ? 'bg-yellow-500 text-white' :
        'bg-blue-500 text-white'
    }`;
    
    // Icono según tipo
    const icon = document.createElement('i');
    icon.className = `mr-2 ${
        type === 'success' ? 'fas fa-check-circle' :
        type === 'error' ? 'fas fa-exclamation-circle' :
        type === 'warning' ? 'fas fa-exclamation-triangle' :
        'fas fa-info-circle'
    }`;
    
    // Mensaje
    const textSpan = document.createElement('span');
    textSpan.textContent = message;
    
    // Añadir elementos al toast
    toast.appendChild(icon);
    toast.appendChild(textSpan);
    
    // Añadir toast al contenedor
    toastContainer.appendChild(toast);
    
    // Eliminar después de 3 segundos
    setTimeout(() => {
        toast.classList.remove('animate-fade-in');
        toast.classList.add('animate-fade-out');
        setTimeout(() => {
            toast.remove();
        }, 300);
    }, 3000);
}
