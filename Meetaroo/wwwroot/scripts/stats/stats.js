'use strict';

const primaryColour = '#202d3d';
const secondaryColour = '#9dd513';

// Bar ------------------------------------------
const viewsGraph = document.getElementById('mt-views-per-day')
Plotly.plot(
    viewsGraph,
    [{
        x: ['2017-10-01', '2017-10-02', '2017-10-03', '2017-10-04', '2017-10-05', '2017-10-06', '2017-10-07', '2017-10-08', '2017-10-09', '2017-10-10', '2017-10-11', '2017-10-12', '2017-10-13', '2017-10-14', '2017-10-15', '2017-10-16', '2017-10-17', '2017-10-18', '2017-10-19', '2017-10-20', '2017-10-21', '2017-10-22', '2017-10-23', '2017-10-24', '2017-10-25', '2017-10-26', '2017-10-27', '2017-10-28', '2017-10-29', '2017-10-30', '2017-10-31'],
        y: [77, 66, 14, 54, 13, 53, 45, 58, 31, 53, 61, 60, 58, 95, 59, 57, 40, 44, 88, 63, 11, 12, 91, 33, 42, 52, 88, 70, 19, 23, 22],
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

// Map ------------------------------------------
const userMap = document.getElementById('mt-user-map');
Plotly.plot(
    userMap,
    [{
        type: 'scattergeo',
        mode: 'markers',
        lat: [36, 47, 42, 27, 31, 40],
        lon: [-117, -123, -74, -81, -102, -111],
        marker: {
            size: [30, 25, 30, 15, 10, 10],
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
            landcolor: primaryColour
        }
    },
    { displayModeBar: false }
);
  