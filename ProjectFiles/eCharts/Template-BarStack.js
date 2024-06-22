    var dom = document.getElementById('container');
    var myChart = echarts.init(dom, 'dark', {
      renderer: 'canvas',
      useDirtyRect: false
    });
    var app = {};
    
    var option;

    // There should not be negative values in rawData
const rawData = [
  [$01, $02, $03, $04],
  [$05, $06, $07, $08],
  [$09, $10, $11, $12],
  [$13, $14, $15, $16],
  [$17, $18, $19, $20],
];
const totalData = [];
for (let i = 0; i < rawData[0].length; ++i) {
  let sum = 0;
  for (let j = 0; j < rawData.length; ++j) {
    sum += rawData[j][i];
  }
  totalData.push(sum);
}
const grid = {
  left: 100,
  right: 100,
  top: 50,
  bottom: 50
};
const series = [
  'Blue Berry',
  'Mint',
  'Mustered',
  'Strawberry',
  'Water'
].map((name, sid) => {
  return {
    name,
    type: 'bar',
    stack: 'total',
    barWidth: '60%',
    label: {
      show: true,
      formatter: (params) => Math.round(params.value * 1000) / 10 + '%'
    },
    data: rawData[sid].map((d, did) =>
      totalData[did] <= 0 ? 0 : d / totalData[did]
    )
  };
});
option = {
  legend: {
    selectedMode: false
  },
  grid,
  yAxis: {
    type: 'value'
  },
  xAxis: {
    type: 'category',
    data: ['7AM-1PM', '1PM-7PM', '7PM-1AM', '1AM-7AM']
  },
  series
};

    if (option && typeof option === 'object') {
      myChart.setOption(option);
    }

    window.addEventListener('resize', myChart.resize);