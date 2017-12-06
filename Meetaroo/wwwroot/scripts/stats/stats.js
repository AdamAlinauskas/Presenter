'use strict';

// const primaryColour = '#202d3d'; // Actual colour from logo
const primaryColour = '#233d5e'; // More vibrant blue
const secondaryColour = '#9dd513';
const colour3 = '#21B5A2';
const colour4 = '#EBE422';

class Plots
{
    showViewsPerDay(model) {
        const viewsGraph = document.getElementById('mt-views-per-day')
        Plotly.plot(
            viewsGraph,
            [{
                x: model.viewsPerDay.map(sample => sample.date),
                y: model.viewsPerDay.map(sample => sample.views),
                type: 'bar',
                marker: {
                    color: primaryColour
                }
            }],
            {
                margin: { l: 50, r: 0, t: 10 },
                xaxis: {
                   tickangle: -45
                }
            },
            { displayModeBar: false }
        );
    }

    showMap() {
        const userMap = document.getElementById('mt-user-map');
        Plotly.plot(
            userMap,
            [{
                type: 'scattergeo',
                mode: 'markers',
                lat: [36, 47, 42, 27, 31, 40, 51, 47],
                lon: [-117, -123, -74, -81, -102, -111, -117, -72],
                marker: {
                    size: [30, 25, 30, 15, 10, 10, 20, 23],
                    color: secondaryColour,
                    opacity: 0.9
                }
            }],
            {
                margin: { l: 0, r: 0, t: 10 },
                geo: {
                    scope: 'north america',
                    lataxis: { 'range': [10, 70] },
                    lonaxis: { 'range': [-130, -55] },
                    showland: true,
                    countrycolor: '#fff',
                    coastlinecolor: '#fff',
                    landcolor: primaryColour
                }
            },
            { displayModeBar: false }
        );
    }

    showViewerTypes() {
        const viewerTypes = document.getElementById('mt-viewer-type');
        Plotly.plot(
            viewerTypes,
            [{
                type: 'pie',
                hole: .6,
                values: [107, 52, 41],
                labels: ['Retail', 'Institutional', 'Other'],
                marker: {
                    colors: [primaryColour, secondaryColour, colour3]
                }
            }],
            {
                margin: { l: 0, r: 0, t: 10 }
            },
            { displayModeBar: false }
        );
    }
}

const plots = new Plots();
fetch('/stats/viewsPerDay', { credentials: 'include' })
    .then(result => result.json()
        .then(result => plots.showViewsPerDay(result))
    )
    .catch(e => {
        console.error(e);
    });
  