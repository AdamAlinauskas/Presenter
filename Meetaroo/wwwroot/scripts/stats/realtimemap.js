'use strict';

// const primaryColour = '#202d3d'; // Actual colour from logo
const primaryColour = '#233d5e'; // More vibrant blue
const secondaryColour = '#9dd513';
const colour3 = '#21B5A2';
const colour4 = '#EBE422';

const svgElement = d3.select('#mapview');
const svgSize = [svgElement.node().offsetWidth, svgElement.node().offsetHeight];
let projection = d3.geoMercator().rotate([100, -45]).scale(600);
const path = d3.geoPath(projection);
const mapGroup = svgElement.append('g');
let features = [];

const appendFeatures = function(error, json) {
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
};

d3.json('/data/NA.geo.json', appendFeatures);