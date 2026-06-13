const f = require('figlet');

// Generate "417" using Slant Relief font (one digit at a time for spacing)
const digits = ['4','1','7'].map(d => f.textSync(d, {font: 'Slant Relief'}).split('\n'));
const clean = digits.map(d => { while(d.length > 0 && d[d.length-1].trim() === '') d.pop(); return d; });
const maxRows = Math.max(...clean.map(d => d.length));
const aligned = clean.map(d => {
  while (d.length < maxRows) d.push(' '.repeat(d[0].length));
  const maxW = Math.max(...d.map(r => r.length));
  return d.map(r => r.padEnd(maxW));
});

const GAP = '        ';
const combined = [];
for (let r = 0; r < maxRows; r++) {
  combined.push(aligned.map(col => col[r]).join(GAP));
}

const colors = [
  [41, 105, 225], [52, 122, 235], [70, 140, 240],
  [100, 110, 235], [145, 82, 215], [185, 60, 180],
  [215, 50, 140], [238, 58, 90], [245, 105, 50],
];

const R = '\x1b[0m';
const B = '\x1b[1m';

console.log('');
combined.forEach((line, i) => {
  const c = colors[Math.min(i, colors.length - 1)];
  console.log(`\x1b[38;2;${c[0]};${c[1]};${c[2]}m${B}${line}${R}`);
});
console.log();

const gray = '\x1b[38;2;130;140;150m';
const green = '\x1b[38;2;80;220;140m';
const blue = '\x1b[38;2;80;180;255m';
const yellow = '\x1b[38;2;255;200;60m';

console.log(`  ${gray}${B}╔══════════════════════════════════════════════════════════════════════════╗${R}`);
console.log(`  ${gray}${B}║${R}  ${green}${B}Author   : James YinG${R}  ${blue}${B}Email : james@taogame.com${R}  ${yellow}${B}业务：软件开发 | 定制 | 修复 | 部署${R}  ${gray}${B}║${R}`);
console.log(`  ${gray}${B}║${R}  ${yellow}${B}专业接单，品质保障，欢迎合作！${R}${gray}${B}║${R}`);
console.log(`  ${gray}${B}╚══════════════════════════════════════════════════════════════════════════╝${R}`);
console.log();
