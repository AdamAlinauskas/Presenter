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

const timeUntilRemoveView = 4000;
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
        const circle = g.append('circle')
            .attr('r', 1)
            .attr('fill', secondaryColour);
        circle.append('animate')
            .attr('id', `anim${view.id}`)
            .attr('attributeName', 'r')
            .attr('attributeType', 'XML')
            .attr('from', 1)
            .attr('to', 30)
            .attr('dur', '.7s')
            .attr('repeatCount', 0)
            .attr('fill', 'freeze')
            .attr('begin', 'indefinite')
            .call(anim => anim.node().beginElement());
        g.append('animate')
            .attr('attributeName', 'opacity')
            .attr('attributeType', 'XML')
            .attr('from', 1)
            .attr('to', 0)
            .attr('dur', '1s')
            .attr('repeatCount', 0)
            .attr('begin', `anim${view.id}.begin+3.1s`);
    });
}

d3.json('/data/NA.geo.json', appendFeatures);