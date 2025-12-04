async function getVideoSources()
{
    const devices = await navigator.mediaDevices.enumerateDevices();
        
    return devices.filter(device => device.kind === 'videoinput');
}

async function getVideoStream(videoSourceDeviceId)
{
    try
    {
        const constraints = { video: { deviceId: { exact: videoSourceDeviceId } } };

        const stream = await navigator.mediaDevices.getUserMedia(constraints);

        return stream;
    }
    catch (error)
    {
        console.error('Error accessing video stream:', error);

        throw error;
    }
}

async function stopVideoStream(stream)
{
    const streamTracks = stream.getTracks();

    streamTracks.forEach(track =>
    {
        track.stop();
    });
}

async function stopVideoStreamById(videoSourceDeviceId)
{
    const constraints = { video: { deviceId: { exact: videoSourceDeviceId } } };

    const stream = await navigator.mediaDevices.getUserMedia(constraints);

    stopVideoStream(stream);
}