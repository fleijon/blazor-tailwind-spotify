export function initAudio(element, reference){
    element.addEventListener("ended", async e => {
        await reference.invokeMethodAsync("OnEndJsFunction");
    });
    element.addEventListener("timeupdate", async e => {
        await reference.invokeMethodAsync("OnTimeUpdateJsFunction", e);
    });
    element.addEventListener("volumechange", async e => {
        await reference.invokeMethodAsync("OnVolumeChangeJsFunction", e);
    })
    element.addEventListener("pause", async e => {
        await reference.invokeMethodAsync("OnPauseJsFunction", e);
    });
    element.addEventListener("play", async e => {
        await reference.invokeMethodAsync("OnPlayJsFunction", e);
    });
}

export function playAudio(element) {
    stopAudio(element);
    element.play();
}

export function stopAudio(element) {
    element.pause();
    element.currentTime = 0;
}

export function pauseAudio(element) {
    element.pause();
}

export function loadAudio(element) {
    element.load();
}

export function getCurrentTime(element) {
    return JSON.stringify(element.currentTime);
}

export function getVolume(element) {
    return JSON.stringify(element.volume);
}

export function setVolume(element, volume) {
    element.volume = volume;
}

export function setTime(element, time) {
    element.currentTime = time;
}
