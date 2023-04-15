let audioPlayer;

class PlaylistItem {
    id;
    audioSource;

    constructor(id, audioSource) {
        this.id = id;
        this.audioSource = audioSource;
    }
}

class AudioPlayer {
    playlist = [];
    index = -1;
    audio;
    dotnetref;

    constructor(dotnetCallbackReference) {
        this.dotnetref = dotnetCallbackReference;
        this.audio = new Audio();

        this.audio.addEventListener("ended", async e => {
            await dotnetCallbackReference.invokeMethodAsync("OnEndJs");
            this.next()
        });
        this.audio.addEventListener("timeupdate", async e => {
            await dotnetCallbackReference.invokeMethodAsync("OnTimeChangedJs", this.audio.currentTime);
        });
        this.audio.addEventListener("volumechange", async e => {
            await dotnetCallbackReference.invokeMethodAsync("OnVolumeChangeJs", e);
        })
        this.audio.addEventListener("pause", async e => {
            await dotnetCallbackReference.invokeMethodAsync("OnPauseJs", e);
        });
        this.audio.addEventListener("play", async e => {
            await dotnetCallbackReference.invokeMethodAsync("OnPlayJs", e);
        });
        this.audio.addEventListener("stop", async e => {
            await dotnetCallbackReference.invokeMethodAsync("OnStopJs", e);
        });
    }

    queueLast(song) {
        this.playlist.push(song);
    }

    queueNext(song) {
        // TODO: Queue the song next in the queue, relative to the current index
    }

    async clearQueue() {
        this.pause();
        this.audio.src = null;
        this.index = -1;

        this.playlist = []

        await this.dotnetref.invokeMethodAsync("AudioUnLoadedJs");
    }

    play() {
        if(this.index == -1 && this.playlist.length > 0) {
            this.index++;
            this.load();
        }

        this.audio.play();
    }

    async load() {
        if(this.index < this.playlist.length && this.index > -1 ){
            var currentItem = this.playlist[this.index];
            this.audio.src = currentItem.audioSource;
            this.audio.load();
            this.dotnetref.invokeMethodAsync("AudioLoadedJs", currentItem);
        }
    }

    next() {
        if(this.index < this.playlist.length-1 ){
            this.index++;
            this.load();
        }

        this.play();
    }

    previous() {
        if(this.index > 0) {
            this.index--;
            this.load();
        }

        this.play();
    }

    pause() {
        this.audio.pause();
    }

    setVolume(val) {
        this.audio.volume = val;
    }

    seek(per) {

        // Convert the percent into a seek position.
        if (this.audio.playing()) {
            this.audio.seek(this.audio.duration() * per);
        }
    }
}

// Public api

export function initializePlayer(dotnetCallbackReference) {
    audioPlayer = new AudioPlayer(dotnetCallbackReference);
}

export function queueLast(song) {
    audioPlayer.queueLast(song);
}

export function queueNext(song) {
    audioPlayer.queueNext(song);
}

export async function clearQueue() {
    await audioPlayer.clearQueue();
}

export function play() {
    audioPlayer.play();
}

export function next() {
    audioPlayer.next();
}

export function previous() {
    audioPlayer.previous();
}

export function pause() {
    audioPlayer.pause();
}

export function stop() {
    audioPlayer.stop();
}

export function seek(position) {
    audioPlayer.seek(position);
}

export function setVolume(vol) {
    audioPlayer.setVolume(vol);
}