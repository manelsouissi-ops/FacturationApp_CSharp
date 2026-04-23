window.initTvaDonutChart = (canvasId, labels, data) => {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    
    // Destroy existing chart if it exists to prevent overlapping on re-renders
    if (window.tvaChartInstance) {
        window.tvaChartInstance.destroy();
    }

    window.tvaChartInstance = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: [
                    '#0d6efd', // blue
                    '#198754', // green
                    '#ffc107', // yellow
                    '#6f42c1', // purple
                    '#fd7e14'  // orange
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom'
                }
            }
        }
    });
};