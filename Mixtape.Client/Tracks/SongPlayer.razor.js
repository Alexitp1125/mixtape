// YouTube player for one song. Keeps the time and the seek bar in sync.
// The audio always comes from the official YouTube player, which stays visible.

let apiLoaded;

// Loads the YouTube IFrame API once for the whole page.
function loadYouTubeApi() {
    apiLoaded ??= new Promise((resolve, reject) => {
        if (window.YT?.Player) {
            resolve();
            return;
        }

        const previous = window.onYouTubeIframeAPIReady;
        window.onYouTubeIframeAPIReady = () => {
            previous?.();
            resolve();
        };

        const script = document.createElement("script");
        script.src = "https://www.youtube.com/iframe_api";
        script.onerror = () => {
            apiLoaded = undefined;
            reject(new Error("The YouTube IFrame API could not be loaded."));
        };
        document.head.appendChild(script);
    });

    return apiLoaded;
}

export async function create(videoHost, timeLabel, seekBar, videoId, dotNet) {
    await loadYouTubeApi();
    return new SongPlayer(videoHost, timeLabel, seekBar, videoId, dotNet);
}

class SongPlayer {
    constructor(videoHost, timeLabel, seekBar, videoId, dotNet) {
        this.timeLabel = timeLabel;
        this.seekBar = seekBar;
        this.dotNet = dotNet;
        // Page element around the cover and the player; gets mx-playing while the song plays.
        this.stage = videoHost.closest(".mx-stage");
        this.playing = false;
        this.dragging = false;

        this.onSeekInput = () => this.seekToBar();
        this.onSeekStart = () => { this.dragging = true; };
        this.onSeekEnd = () => { this.dragging = false; };
        seekBar.addEventListener("input", this.onSeekInput);
        seekBar.addEventListener("pointerdown", this.onSeekStart);
        seekBar.addEventListener("change", this.onSeekEnd);

        // YouTube replaces this element with its iframe; Blazor keeps owning videoHost.
        const target = document.createElement("div");
        videoHost.appendChild(target);

        // videoId without autoplay: the video is cued and waits for play.
        this.player = new YT.Player(target, {
            host: "https://www.youtube-nocookie.com",
            videoId,
            width: "100%",
            height: "100%",
            playerVars: { playsinline: 1, rel: 0, origin: location.origin },
            events: {
                onReady: () => this.notify("OnReady"),
                onStateChange: e => this.onStateChange(e.data),
                onError: e => this.notify("OnError", e.data),
            },
        });

        this.timer = setInterval(() => this.tick(), 250);
    }

    toggle() {
        if (this.player.getPlayerState?.() === YT.PlayerState.PLAYING) {
            this.player.pauseVideo();
        } else {
            this.player.playVideo?.();
        }
    }

    dispose() {
        clearInterval(this.timer);
        this.seekBar.removeEventListener("input", this.onSeekInput);
        this.seekBar.removeEventListener("pointerdown", this.onSeekStart);
        this.seekBar.removeEventListener("change", this.onSeekEnd);
        this.stage?.classList.remove("mx-playing");
        this.player.destroy?.();
    }

    tick() {
        const duration = this.player.getDuration?.();
        if (!duration) {
            return;
        }

        const now = this.player.getCurrentTime();

        if (!this.dragging) {
            const progress = now / duration * 1000;
            this.seekBar.value = progress;
            this.seekBar.style.setProperty("--progress", `${progress / 10}%`);
        }

        this.timeLabel.textContent = `${formatTime(now)} / ${formatTime(duration)}`;
    }

    seekToBar() {
        const duration = this.player.getDuration?.();
        if (!duration) {
            return;
        }

        this.seekBar.style.setProperty("--progress", `${this.seekBar.value / 10}%`);
        this.player.seekTo(duration * this.seekBar.value / 1000, true);
    }

    onStateChange(state) {
        const playing = state === YT.PlayerState.PLAYING;
        if (playing === this.playing) {
            return;
        }

        this.playing = playing;
        this.stage?.classList.toggle("mx-playing", playing);
        this.notify("OnPlayingChanged", playing);
    }

    // Calls the Blazor component. Ignores the call if the component is already gone.
    notify(method, ...args) {
        this.dotNet.invokeMethodAsync(method, ...args).catch(() => { });
    }
}

function formatTime(seconds) {
    const total = Math.floor(seconds || 0);
    return `${Math.floor(total / 60)}:${String(total % 60).padStart(2, "0")}`;
}
