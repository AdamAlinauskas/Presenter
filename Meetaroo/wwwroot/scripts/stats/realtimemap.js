'use strict';

// TODO AP : All this is global. Move to a module, or at least an onload function

// const primaryColour = '#202d3d'; // Actual colour from logo
const primaryColour = '#233d5e'; // More vibrant blue
const secondaryColour = '#9dd513';
const colour3 = '#21B5A2';
const colour4 = '#EBE422';

const svgElement = d3.select('#mapview');
const svgSize = [svgElement.node().offsetWidth, svgElement.node().offsetHeight];
let projection = d3.geoMercator().rotate([100, -45]).scale(800);
const path = d3.geoPath(projection);
const mapGroup = svgElement.append('g');
let features = [];
let lastViewId = 0;
let fetchTimeoutId = null;

function appendFeatures (error, json) {
    if (error) throw error;

    json.forEach(
        country => country.features.forEach(feature => {
            features.push(feature);
        })
    );

    mapGroup
        .selectAll('path')
        .data(features)
        .enter()
        .append('path')
        .attr('d', path)
        .style('fill', primaryColour)
        .style('stroke', 'white');

    fetchTimeoutId = setInterval(fetchViews, 1000);
};

function fetchViews() {
    fetch(`/stats/realtimeStats/${lastViewId}`, { credentials: 'include' })
        .then(data => data.json().then(displayViews))
        .catch(error => {
            console.error(error);
            clearInterval(fetchTimeoutId);
        });
}

const timeUntilAnimateOut = 5000;
const timeUntilRemoveView = timeUntilAnimateOut + 1000;
const labelWidth = 150;
const iconRadius = 20;
const iconDiameter = iconRadius * 2;
function displayViews(views) {
    views.samples.forEach(view => {
        lastViewId = Math.max(lastViewId, view.id);
        
        const location = projection([view.long, view.lat]);
    
        const g = svgElement.append('g')
            .attr("transform", d => {
                return `translate(${location[0]}, ${location[1]})`;
            })
            .call(g => {
                setTimeout(g.remove.bind(g), timeUntilRemoveView);
            });
        g.append('clipPath')
            .attr('id', `label-clip-${view.id}`)
            .append('rect')
                .classed('animated', true)
                .classed('slideInLeft', true)
                .attr('x', 0)
                .attr('y', -iconRadius)
                .attr('height', iconRadius)
                .attr('width', labelWidth)
                .attr('preserveAspectRatio', 'xMidYMid slice')
                .call(elem => {
                    setTimeout(() => elem.classed('slideOutLeft', true), timeUntilAnimateOut);
                })
        g.append('rect')
            .attr('clip-path', `url(#label-clip-${view.id})`)
            .attr('x', 0)
            .attr('y', -iconRadius)
            .attr('height', iconRadius)
            .attr('width', labelWidth)
            .attr('fill', secondaryColour)
        g.append('text')
            .attr('clip-path', `url(#label-clip-${view.id})`)
            .text(view.name)
            .classed('view-label', true)
            .attr('x', iconRadius)
            .attr('y', -5)
            .attr('fill', 'white')
        g.append('circle')
            .attr('r', iconRadius)
            .attr('fill', secondaryColour)
            .classed('animated', true)
            .classed('zoomIn', true)
            .call(elem => {
                setTimeout(() => elem.classed('zoomOut', true), timeUntilAnimateOut);
            });
        g.append('clipPath')
            .attr('id', `profile-clip-${view.id}`)
            .append('circle')
                .attr('r', iconRadius - 2)
        g.append('image')
            .attr('clip-path', `url(#profile-clip-${view.id})`)
            .attr('x', -20)
            .attr('y', -20)
            .attr('width', iconDiameter)
            .attr('height', iconDiameter)
            .attr('href', 'https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_960_720.png')
            .classed('animated', true)
            .classed('zoomIn', true)
            .call(elem => {
                setTimeout(() => elem.classed('zoomOut', true), timeUntilAnimateOut);
            });
    });
}

d3.json('/data/NA.geo.json', appendFeatures);