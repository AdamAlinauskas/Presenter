'use strict';

// const primaryColour = '#202d3d'; // Actual colour from logo
const primaryColour = '#233d5e'; // More vibrant blue
const secondaryColour = '#9dd513';
const colour3 = '#21B5A2';
const colour4 = '#EBE422';
const d3 = Plotly.d3;

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

    showMap(model) {
        const views = model.samples.map(sample => sample.views);
        const sizeScale = d3.scale
            .linear()
            .domain([d3.min(views), d3.max(views)])
            .range([5, 30]);

        const userMap = document.getElementById('mt-user-map');
        Plotly.plot(
            userMap,
            [{
                type: 'scattergeo',
                mode: 'markers',
                lat: model.samples.map(sample => sample.centroidLat),
                lon: model.samples.map(sample => sample.centroidLong),
                text: views.map(sample => sample + sample === 1 ? ' viewer' : ' viewers'),
                hoverinfo: 'text',
                marker: {
                    size: views.map(sizeScale),
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

fetch('/stats/geographicViews', { credentials: 'include' })
    .then(result => result.json()
        .then(result => plots.showMap(result))
    )
    .catch(e => {
        console.error(e);
    });
