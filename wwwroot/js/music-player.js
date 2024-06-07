document.addEventListener('DOMContentLoaded', (event) => {
    const audioPlayer = document.getElementById('audio-player');
    const playButton = document.getElementById('play-button');
    const pauseButton = document.getElementById('pause-button');
    const seekSlider = document.getElementById('seek-slider');
    const volumeSlider = document.getElementById('volume-slider');
    const prevButton = document.getElementById('prev-button');
    const nextButton = document.getElementById('next-button');
    const replayButton = document.getElementById('replay-button');
    const muteButton = document.getElementById('mute-button');
    const currentTimeSpan = document.getElementById('current-time');
    const totalTimeSpan = document.getElementById('total-time');
    const volumeLevelSpan = document.getElementById('volume-level');
    const currentSongTitleSpan = document.getElementById('current-song-title');
    const musicPlayerFooter = document.getElementById('music-player-footer');
    const musicPlayerPlaceholder = document.getElementById('music-player-placeholder');
    let currentSongIndex = -1;
    let songs = [];
    let songTitles = [];
    let isSeeking = false;
    let wasPlayingBeforeSeek = false;
    let currentPlayButton = null;
    let isMuted = false;
    let isRepeating = false;

    // Set initial volume to 50%
    audioPlayer.volume = 0.5;

    document.querySelectorAll('.play-button').forEach((button, index) => {
        button.addEventListener('click', () => {
            songs = Array.from(document.querySelectorAll('.play-button')).map(b => b.getAttribute('data-song-url'));
            songTitles = Array.from(document.querySelectorAll('.play-button')).map(b => b.getAttribute('data-song-title'));
            currentSongIndex = index;
            playSong(button);
        });
    });

    playButton.addEventListener('click', () => {
        audioPlayer.play();
        playButton.style.display = 'none';
        pauseButton.style.display = 'inline-block';
        updatePlayButton('Playing');
    });

    pauseButton.addEventListener('click', () => {
        audioPlayer.pause();
        playButton.style.display = 'inline-block';
        pauseButton.style.display = 'none';
        updatePlayButton('Play');
    });

    prevButton.addEventListener('click', () => {
        if (currentSongIndex > 0) {
            currentSongIndex--;
        } else {
            currentSongIndex = songs.length - 1; // Loop to the last song
        }
        playSong();
    });

    nextButton.addEventListener('click', () => {
        if (currentSongIndex < songs.length - 1) {
            currentSongIndex++;
        } else {
            currentSongIndex = 0; // Loop to the first song
        }
        playSong();
    });

    replayButton.addEventListener('click', () => {
        isRepeating = !isRepeating;
        replayButton.innerHTML = isRepeating ? '<i class="fas fa-redo"></i> <span class="d-none">Repeat On</span>' : '<i class="fas fa-times"></i> <span class="d-none">Repeat Off</span>';
    });

    muteButton.addEventListener('click', () => {
        isMuted = !isMuted;
        audioPlayer.muted = isMuted;
        muteButton.innerHTML = isMuted ? '<i class="fas fa-volume-mute"></i>' : '<i class="fas fa-volume-up"></i>';
        volumeSlider.value = isMuted ? 0 : audioPlayer.volume * 100;
        volumeLevelSpan.textContent = isMuted ? '0%' : `${volumeSlider.value}%`;
    });

    audioPlayer.addEventListener('timeupdate', () => {
        if (!isSeeking) {
            seekSlider.value = (audioPlayer.currentTime / audioPlayer.duration) * 100;
            currentTimeSpan.textContent = formatTime(audioPlayer.currentTime);
        }
    });

    audioPlayer.addEventListener('loadedmetadata', () => {
        totalTimeSpan.textContent = formatTime(audioPlayer.duration);
    });

    audioPlayer.addEventListener('ended', () => {
        if (isRepeating) {
            audioPlayer.currentTime = 0;
            audioPlayer.play();
        } else {
            nextButton.click(); // Play the next song
        }
    });

    seekSlider.addEventListener('mousedown', (e) => {
        if (e.button === 0) { // Only act on left click
            isSeeking = true;
            wasPlayingBeforeSeek = !audioPlayer.paused;
            if (wasPlayingBeforeSeek) {
                audioPlayer.pause();
            }
            console.log('Seeking started');
        }
    });

    seekSlider.addEventListener('mouseup', (e) => {
        if (e.button === 0) { // Only act on left click
            isSeeking = false;
            const seekTo = audioPlayer.duration * (e.target.value / 100);
            audioPlayer.currentTime = seekTo;
            if (wasPlayingBeforeSeek) {
                audioPlayer.play();
            }
            console.log('Seeking ended: ', seekTo, 'Current time:', audioPlayer.currentTime);
        }
    });

    seekSlider.addEventListener('input', (e) => {
        if (isSeeking) {
            const seekTo = audioPlayer.duration * (e.target.value / 100);
            currentTimeSpan.textContent = formatTime(seekTo);
            console.log('Seeking to: ', seekTo, 'Displayed time:', formatTime(seekTo));
        }
    });

    // Prevent right-click interaction for the entire music player
    musicPlayerFooter.addEventListener('contextmenu', (e) => {
        e.preventDefault();
    });

    volumeSlider.addEventListener('input', () => {
        const volume = volumeSlider.value / 100;
        audioPlayer.volume = volume;
        audioPlayer.muted = volume === 0;
        muteButton.innerHTML = volume === 0 ? '<i class="fas fa-volume-mute"></i>' : '<i class="fas fa-volume-up"></i>';
        volumeLevelSpan.textContent = `${volumeSlider.value}%`;
    });

    function playSong(button = null) {
        if (currentPlayButton) {
            currentPlayButton.textContent = 'Play';
        }

        if (button) {
            currentPlayButton = button;
            currentPlayButton.textContent = 'Playing';
        } else {
            const buttons = document.querySelectorAll('.play-button');
            currentPlayButton = buttons[currentSongIndex];
            currentPlayButton.textContent = 'Playing';
        }

        currentSongTitleSpan.textContent = `Now Playing: ${songTitles[currentSongIndex]}`;

        audioPlayer.src = songs[currentSongIndex];
        audioPlayer.load(); // Ensure the audio is loaded before playing
        audioPlayer.play().then(() => {
            musicPlayerFooter.style.display = 'flex';
            musicPlayerPlaceholder.style.display = 'none';
            playButton.style.display = 'none';
            pauseButton.style.display = 'inline-block';
        }).catch((error) => {
            console.error('Error playing audio:', error);
        });
    }

    function updatePlayButton(text) {
        if (currentPlayButton) {
            currentPlayButton.textContent = text;
        }
    }

    function formatTime(seconds) {
        const minutes = Math.floor(seconds / 60);
        const secs = Math.floor(seconds % 60);
        return `${minutes}:${secs < 10 ? '0' : ''}${secs}`;
    }
});
