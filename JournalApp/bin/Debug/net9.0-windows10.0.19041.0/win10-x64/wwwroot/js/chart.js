window.renderMoodDistributionChart = (labels, data) => {
    const ctx = document.getElementById('moodDistributionChart');
    if (!ctx) return;

    // Destroy existing chart if it exists
    if (window.moodDistChart) {
        window.moodDistChart.destroy();
    }

    window.moodDistChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: [
                    '#4CAF50',  // Positive - Green
                    '#FFC107',  // Neutral - Yellow
                    '#F44336'   // Negative - Red
                ],
                borderWidth: 2,
                borderColor: '#fff'
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 20,
                        font: {
                            size: 14
                        }
                    }
                }
            }
        }
    });
};

window.renderFrequentMoodsChart = (labels, data) => {
    const ctx = document.getElementById('frequentMoodsChart');
    if (!ctx) return;

    if (window.freqMoodsChart) {
        window.freqMoodsChart.destroy();
    }

    window.freqMoodsChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Frequency',
                data: data,
                backgroundColor: 'rgba(54, 162, 235, 0.6)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    });
};

window.renderTagsChart = (labels, data) => {
    const ctx = document.getElementById('tagsChart');
    if (!ctx) return;

    if (window.tagsChart) {
        window.tagsChart.destroy();
    }

    window.tagsChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Usage Count',
                data: data,
                backgroundColor: 'rgba(153, 102, 255, 0.6)',
                borderColor: 'rgba(153, 102, 255, 1)',
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            indexAxis: 'y',
            scales: {
                x: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    });
};

window.renderWordCountChart = (labels, wordCounts, averages) => {
    const ctx = document.getElementById('wordCountChart');
    if (!ctx) return;

    if (window.wordCountChart) {
        window.wordCountChart.destroy();
    }

    window.wordCountChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Word Count',
                    data: wordCounts,
                    borderColor: 'rgba(75, 192, 192, 1)',
                    backgroundColor: 'rgba(75, 192, 192, 0.2)',
                    borderWidth: 2,
                    tension: 0.4
                },
                {
                    label: 'Average',
                    data: averages,
                    borderColor: 'rgba(255, 99, 132, 1)',
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    borderWidth: 2,
                    borderDash: [5, 5],
                    tension: 0.4
                }
            ]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true
                }
            },
            plugins: {
                legend: {
                    position: 'bottom'
                }
            }
        }
    });
};