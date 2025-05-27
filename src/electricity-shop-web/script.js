const canvas = document.getElementById('gameCanvas');
const ctx = canvas.getContext('2d');
const scoreElement = document.getElementById('score');
const levelElement = document.getElementById('level');

let score = 0;
let level = 1;
let ducks = [];

function initializeGame() {
    canvas.width = 800; // Set canvas width to match container
    canvas.height = 600; // Set canvas height to match container
    score = 0;
    level = 1;
    ducks = [];
    createDucks();
    gameLoop();
}

function createDucks() {
    // For now, create a single duck for testing
    ducks.push({
        x: 100,
        y: 500,
        xVelocity: 2,
        yVelocity: -2,
        width: 50,
        height: 50,
        isAlive: true
    });
}

function updateDuck(duck) {
    if (!duck.isAlive) return;

    duck.x += duck.xVelocity;
    duck.y += duck.yVelocity;

    // Bounce off the edges
    if (duck.x + duck.width > canvas.width || duck.x < 0) {
        duck.xVelocity *= -1;
    }
    if (duck.y + duck.height > canvas.height || duck.y < 0) {
        duck.yVelocity *= -1;
    }
}
function drawDuck(duck){
    if(!duck.isAlive) return;
     ctx.fillStyle = 'brown';
     ctx.fillRect(duck.x, duck.y, duck.width, duck.height);
}

function updateGame() {
    ducks.forEach(updateDuck);
}

function drawGame() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ducks.forEach(drawDuck);
    scoreElement.textContent = score;
    levelElement.textContent = level;

}

function gameLoop() {
    updateGame();
    drawGame();
    requestAnimationFrame(gameLoop);
}

canvas.addEventListener('click', (event) => {
    const rect = canvas.getBoundingClientRect();
    const clickX = event.clientX - rect.left;
    const clickY = event.clientY - rect.top;

    ducks.forEach(duck => {
        if (clickX >= duck.x && clickX <= duck.x + duck.width &&
            clickY >= duck.y && clickY <= duck.y + duck.height) {
            if(duck.isAlive){
                score += 10;
                duck.isAlive = false;
            }
        }
    });
});

initializeGame();
