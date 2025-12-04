function seedRandom(seed)
{
    const randomValue = Math.sin(seed) * 10000;

    return randomValue - Math.floor(randomValue);
}

function generateColorDeterministically(seed)
{
    const redColorValue = seedRandom(seed);
    const greenColorValue = seedRandom(seed + 1);
    const blueColorValue = seedRandom(seed + 2);

    const red = Math.floor(redColorValue * 256);
    const green = Math.floor(greenColorValue * 256);
    const blue = Math.floor(blueColorValue * 256);

    return `rgb(${red}, ${green}, ${blue})`;
}
