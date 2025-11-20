import re
import json
import matplotlib.pyplot as plt

# IDE másold be a teljes logot!
raw = r"""
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.375075,"accelY":-0.820203,"accelZ":0,"gyrosX":-5.418352,"gyrosY":28.72306,"gyrosZ":20.81193,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.381132,"accelY":-0.820748,"accelZ":0,"gyrosX":-6.448403,"gyrosY":26.01052,"gyrosZ":11.65448,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.377534,"accelY":-0.844421,"accelZ":0,"gyrosX":-14.50684,"gyrosY":4.623187,"gyrosZ":13.65343,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.39633,"accelY":-0.86275,"accelZ":0,"gyrosX":-17.66073,"gyrosY":7.207782,"gyrosZ":-2.319834,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[634232][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.277449,"accelY":-0.603943,"accelZ":0,"gyrosX":-12.13257,"gyrosY":4.331502,"gyrosZ":-1.743862,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[634411][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.194233,"accelY":-0.422779,"accelZ":0,"gyrosX":-8.262859,"gyrosY":2.318107,"gyrosZ":-1.340682,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[634580][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.135981,"accelY":-0.295963,"accelZ":0,"gyrosX":-5.554062,"gyrosY":0.90873,"gyrosZ":-1.058456,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[634731][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":-0.207193,"accelZ":0,"gyrosX":-3.657904,"gyrosY":0,"gyrosZ":-0.860898,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[634901][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":-0.145053,"accelZ":0,"gyrosX":-2.330593,"gyrosY":-0.713945,"gyrosZ":-0.722607,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[635073][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":-0.101556,"accelZ":0,"gyrosX":-1.401476,"gyrosY":-1.213706,"gyrosZ":-0.625804,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[635252][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":0,"accelZ":0,"gyrosX":-0.751094,"gyrosY":-1.563539,"gyrosZ":-0.558042,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[635410][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":0,"accelZ":0,"gyrosX":0,"gyrosY":-1.808422,"gyrosZ":-0.510608,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.134326,"accelY":-0.271875,"accelZ":0,"gyrosX":17.28643,"gyrosY":4.162145,"gyrosZ":5.930229,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.226743,"accelY":-0.455522,"accelZ":0,"gyrosX":15.11059,"gyrosY":5.914061,"gyrosZ":11.29072,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.298466,"accelY":-0.579242,"accelZ":0,"gyrosX":12.01651,"gyrosY":1.726661,"gyrosZ":16.56368,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.344132,"accelY":-0.665845,"accelZ":0,"gyrosX":11.49493,"gyrosY":0,"gyrosZ":16.51735,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.391991,"accelY":-0.739578,"accelZ":0,"gyrosX":6.632118,"gyrosY":0,"gyrosZ":12.06506,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.423954,"accelY":-0.770097,"accelZ":0,"gyrosX":2.999139,"gyrosY":-10.0941,"gyrosZ":-0.935504,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.405386,"accelY":-0.781353,"accelZ":0,"gyrosX":-12.07524,"gyrosY":-22.65157,"gyrosZ":-21.84353,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.340972,"accelY":-0.794579,"accelZ":0,"gyrosX":-11.07159,"gyrosY":-22.50134,"gyrosZ":-30.43793,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.350009,"accelY":-0.800395,"accelZ":0,"gyrosX":-1.570552,"gyrosY":-5.023661,"gyrosZ":-2.533403,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.34088,"accelY":-0.79802,"accelZ":0,"gyrosX":2.716813,"gyrosY":19.09621,"gyrosZ":14.99824,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.398284,"accelY":-0.795919,"accelZ":0,"gyrosX":1.838578,"gyrosY":26.66867,"gyrosZ":18.00933,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.415981,"accelY":-0.828213,"accelZ":0,"gyrosX":-4.414353,"gyrosY":19.20909,"gyrosZ":20.10792,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.421265,"accelY":-0.845325,"accelZ":0,"gyrosX":-9.093696,"gyrosY":10.37822,"gyrosZ":23.4777,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.451038,"accelY":-0.86265,"accelZ":0,"gyrosX":-7.239464,"gyrosY":6.450044,"gyrosZ":1.795328,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.40911,"accelY":-0.874265,"accelZ":0,"gyrosX":-12.65601,"gyrosY":4.964445,"gyrosZ":-15.29226,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.40774,"accelY":-0.9218,"accelZ":0,"gyrosX":-20.14377,"gyrosY":-6.962499,"gyrosZ":-11.03067,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.409124,"accelY":-0.936544,"accelZ":0,"gyrosX":-25.67375,"gyrosY":-2.060976,"gyrosZ":-19.84603,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[638381][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.286405,"accelY":-0.655599,"accelZ":0,"gyrosX":-17.74169,"gyrosY":-2.156628,"gyrosZ":-14.0122,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[638559][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.200502,"accelY":-0.458938,"accelZ":0,"gyrosX":-12.18924,"gyrosY":-2.223584,"gyrosZ":-9.928516,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.229267,"accelY":-0.620817,"accelZ":0,"gyrosX":-9.255201,"gyrosY":-3.236866,"gyrosZ":-6.158489,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.258119,"accelY":-0.755959,"accelZ":0,"gyrosX":1.116184,"gyrosY":-3.923263,"gyrosZ":0,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.295747,"accelY":-0.8217,"accelZ":0,"gyrosX":18.76394,"gyrosY":-0.973206,"gyrosZ":6.883075,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.320914,"accelY":-0.86208,"accelZ":0,"gyrosX":26.23493,"gyrosY":7.783438,"gyrosZ":6.154662,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.319269,"accelY":-0.871522,"accelZ":0,"gyrosX":29.09668,"gyrosY":9.424538,"gyrosZ":11.15928,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.364699,"accelY":-0.90428,"accelZ":0,"gyrosX":25.59914,"gyrosY":2.333613,"gyrosZ":20.96938,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.392765,"accelY":-0.911536,"accelZ":0,"gyrosX":17.88369,"gyrosY":-16.51706,"gyrosZ":29.26546,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.385238,"accelY":-0.877284,"accelZ":0,"gyrosX":-0.643847,"gyrosY":-41.33237,"gyrosZ":13.99027,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.468226,"accelY":-0.81698,"accelZ":0,"gyrosX":-11.14442,"gyrosY":-69.53516,"gyrosZ":0,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.390088,"accelY":-0.814024,"accelZ":0,"gyrosX":3.265487,"gyrosY":-44.43283,"gyrosZ":0.988418,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.42709,"accelY":-0.812469,"accelZ":0,"gyrosX":-2.114754,"gyrosY":-25.32685,"gyrosZ":13.00245,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.422376,"accelY":-0.805081,"accelZ":0,"gyrosX":12.50839,"gyrosY":-6.204573,"gyrosZ":28.52983,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.443246,"accelY":-0.809137,"accelZ":0,"gyrosX":15.97971,"gyrosY":9.81461,"gyrosZ":38.17151,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.461224,"accelY":-0.826919,"accelZ":0,"gyrosX":11.67222,"gyrosY":3.284527,"gyrosZ":41.69626,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[640972][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.322875,"accelY":-0.578861,"accelZ":0,"gyrosX":8.400495,"gyrosY":1.585224,"gyrosZ":29.06741,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[641151][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.226031,"accelY":-0.405221,"accelZ":0,"gyrosX":6.110286,"gyrosY":0,"gyrosZ":20.22721,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.263031,"accelY":-0.55304,"accelZ":0,"gyrosX":0,"gyrosY":-0.929212,"gyrosZ":-5.394523,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.285929,"accelY":-0.713641,"accelZ":0,"gyrosX":-5.742579,"gyrosY":-4.240729,"gyrosZ":-18.64424,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[641654][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.200168,"accelY":-0.499567,"accelZ":0,"gyrosX":-3.789866,"gyrosY":-3.682455,"gyrosZ":-13.17094,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.239434,"accelY":-0.643032,"accelZ":0,"gyrosX":-8.871821,"gyrosY":-2.105404,"gyrosZ":-14.42361,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.251686,"accelY":-0.741773,"accelZ":0,"gyrosX":-2.256671,"gyrosY":1.732884,"gyrosZ":-12.58902,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.252572,"accelY":-0.819021,"accelZ":0,"gyrosX":-1.720723,"gyrosY":4.29144,"gyrosZ":-9.871226,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[642327][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.176819,"accelY":-0.573333,"accelZ":0,"gyrosX":-0.974566,"gyrosY":2.290063,"gyrosZ":-7.029837,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[642508][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.123791,"accelY":-0.401352,"accelZ":0,"gyrosX":0,"gyrosY":0.8891,"gyrosZ":-5.040865,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[642687][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":-0.280964,"accelZ":0,"gyrosX":0,"gyrosY":0,"gyrosZ":-3.648584,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[642855][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":-0.196693,"accelZ":0,"gyrosX":0,"gyrosY":-0.713945,"gyrosZ":-2.673988,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[643023][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":-0.137704,"accelZ":0,"gyrosX":0,"gyrosY":-1.213706,"gyrosZ":-1.99177,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[643183][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":0,"accelZ":0,"gyrosX":0,"gyrosY":-1.563539,"gyrosZ":-1.514218,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.283154,"accelZ":0,"gyrosX":0,"gyrosY":-1.803842,"gyrosZ":-1.175351,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.547647,"accelZ":-0.145972,"gyrosX":-27.66777,"gyrosY":18.56535,"gyrosZ":-17.59616,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.677713,"accelZ":-0.235041,"gyrosX":-16.41231,"gyrosY":19.55508,"gyrosZ":-18.69378,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.751547,"accelZ":-0.294021,"gyrosX":-13.94723,"gyrosY":15.96087,"gyrosZ":-12.24837,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.805649,"accelZ":-0.338017,"gyrosX":-13.52243,"gyrosY":15.72584,"gyrosZ":-5.735061,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.909218,"accelZ":-0.393277,"gyrosX":7.605458,"gyrosY":-5.484482,"gyrosZ":8.79067,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.951101,"accelZ":-0.353883,"gyrosX":44.68201,"gyrosY":-27.9256,"gyrosZ":22.76021,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.993749,"accelZ":-0.271229,"gyrosX":79.2646,"gyrosY":-27.155,"gyrosZ":44.46103,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.967939,"accelZ":-0.142839,"gyrosX":87.82661,"gyrosY":-14.08427,"gyrosZ":39.16457,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.965106,"accelZ":0,"gyrosX":60.09177,"gyrosY":-5.873699,"gyrosZ":39.18072,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.10459,"accelY":-0.946058,"accelZ":0,"gyrosX":40.86517,"gyrosY":-2.352252,"gyrosZ":34.47446,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.197065,"accelY":-0.939316,"accelZ":0,"gyrosX":23.79739,"gyrosY":-11.53915,"gyrosZ":22.28543,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.205622,"accelY":-0.886476,"accelZ":0,"gyrosX":2.648417,"gyrosY":-27.50127,"gyrosZ":3.5073,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.22582,"accelY":-0.839967,"accelZ":0,"gyrosX":0,"gyrosY":-16.61216,"gyrosZ":0,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.261419,"accelY":-0.844984,"accelZ":0,"gyrosX":-8.14716,"gyrosY":-8.664597,"gyrosZ":13.58842,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.364707,"accelY":-0.798984,"accelZ":0,"gyrosX":0.613951,"gyrosY":4.483434,"gyrosZ":30.54306,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.426755,"accelY":-0.816662,"accelZ":0,"gyrosX":0,"gyrosY":14.24583,"gyrosZ":29.64184,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.393284,"accelY":-0.836434,"accelZ":0,"gyrosX":-8.389908,"gyrosY":21.48257,"gyrosZ":15.54992,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.397101,"accelY":-0.841851,"accelZ":0,"gyrosX":-7.465897,"gyrosY":20.96049,"gyrosZ":-7.038089,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.378092,"accelY":-0.885853,"accelZ":0,"gyrosX":-23.33512,"gyrosY":-2.662974,"gyrosZ":-17.95809,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.328459,"accelY":-0.95291,"accelZ":0,"gyrosX":-18.74739,"gyrosY":-15.70475,"gyrosZ":-28.1166,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[646782][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.229939,"accelY":-0.667055,"accelZ":0,"gyrosX":-12.89324,"gyrosY":-11.70727,"gyrosZ":-19.8016,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[646950][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.160976,"accelY":-0.466957,"accelZ":0,"gyrosX":-8.795325,"gyrosY":-8.90903,"gyrosZ":-13.9811,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[647118][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.112701,"accelY":-0.326888,"accelZ":0,"gyrosX":-5.926788,"gyrosY":-6.950266,"gyrosZ":-9.906746,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[647287][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":-0.22884,"accelZ":0,"gyrosX":-3.918812,"gyrosY":-5.579131,"gyrosZ":-7.054701,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[647450][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":-0.160206,"accelZ":0,"gyrosX":-2.513229,"gyrosY":-4.619337,"gyrosZ":-5.05827,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[647611][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":-0.112163,"accelZ":0,"gyrosX":-1.529321,"gyrosY":-3.94748,"gyrosZ":-3.660767,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[647781][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":0,"accelZ":0,"gyrosX":-0.840585,"gyrosY":-3.477181,"gyrosZ":-2.682516,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[647959][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":0,"accelZ":0,"gyrosX":0,"gyrosY":-3.147972,"gyrosZ":-1.99774,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[648112][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":0,"accelZ":0,"gyrosX":0,"gyrosY":-2.917525,"gyrosZ":-1.518397,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[648301][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":0,"accelZ":0,"gyrosX":0,"gyrosY":-2.756212,"gyrosZ":-1.182856,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.262573,"accelZ":-0.127808,"gyrosX":0,"gyrosY":-2.638713,"gyrosZ":-0.943398,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.398914,"accelZ":-0.305457,"gyrosX":-65.35327,"gyrosY":-13.07708,"gyrosZ":-10.54524,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.46923,"accelZ":-0.49485,"gyrosX":-100.0166,"gyrosY":-14.10912,"gyrosZ":-8.605466,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.525116,"accelZ":-0.696712,"gyrosX":-62.37099,"gyrosY":-2.712466,"gyrosZ":-10.31174,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.572879,"accelZ":-0.805203,"gyrosX":21.77331,"gyrosY":20.71863,"gyrosZ":-3.953466,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.659634,"accelZ":-0.805415,"gyrosX":85.0392,"gyrosY":23.14177,"gyrosZ":-0.986642,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.75149,"accelZ":-0.674606,"gyrosX":135.852,"gyrosY":16.51125,"gyrosZ":8.931357,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.843035,"accelZ":-0.482039,"gyrosX":153.4073,"gyrosY":19.10194,"gyrosZ":19.14877,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.854016,"accelZ":-0.254444,"gyrosX":150.728,"gyrosY":33.54749,"gyrosZ":37.86126,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.841854,"accelZ":0,"gyrosX":132.0571,"gyrosY":33.74335,"gyrosZ":35.59359,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.890543,"accelZ":0.118872,"gyrosX":93.1142,"gyrosY":10.06823,"gyrosZ":54.54821,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.866178,"accelZ":0.168391,"gyrosX":29.74423,"gyrosY":-11.91351,"gyrosZ":32.01796,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.815504,"accelZ":0.232058,"gyrosX":25.69517,"gyrosY":-24.74959,"gyrosZ":-14.87534,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.775711,"accelZ":0.308486,"gyrosX":13.32954,"gyrosY":-21.56537,"gyrosZ":3.899343,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.112866,"accelY":-0.76646,"accelZ":0.308591,"gyrosX":5.044586,"gyrosY":2.900221,"gyrosZ":32.59582,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.201394,"accelY":-0.791039,"accelZ":0.244725,"gyrosX":-14.33504,"gyrosY":6.084149,"gyrosZ":36.19022,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.269809,"accelY":-0.815348,"accelZ":0.203241,"gyrosX":-9.332829,"gyrosY":8.615189,"gyrosZ":21.2147,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.275951,"accelY":-0.839176,"accelZ":0.168636,"gyrosX":-8.116781,"gyrosY":1.858672,"gyrosZ":16.2326,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.307057,"accelY":-0.843478,"accelZ":0.19158,"gyrosX":-9.207533,"gyrosY":7.544378,"gyrosZ":-2.231965,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.310302,"accelY":-0.840996,"accelZ":0.203686,"gyrosX":-21.25656,"gyrosY":6.07857,"gyrosZ":-16.119,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.309276,"accelY":-0.910816,"accelZ":0.163674,"gyrosX":-29.88782,"gyrosY":-1.423832,"gyrosZ":-32.75137,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.253041,"accelY":-0.96013,"accelZ":0.114938,"gyrosX":-30.49306,"gyrosY":-5.278566,"gyrosZ":-24.22303,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.233745,"accelY":-0.9811,"accelZ":0,"gyrosX":-35.10757,"gyrosY":0,"gyrosZ":-33.22572,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.211669,"accelY":-1.024197,"accelZ":0,"gyrosX":-20.62627,"gyrosY":0.586819,"gyrosZ":-27.34897,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.182299,"accelY":-1.011371,"accelZ":0,"gyrosX":-11.87715,"gyrosY":0,"gyrosZ":-18.30701,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.171188,"accelY":-1.01338,"accelZ":0,"gyrosX":-10.27796,"gyrosY":-0.691044,"gyrosZ":-14.00206,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.164436,"accelY":-1.046573,"accelZ":0,"gyrosX":4.142236,"gyrosY":-3.71218,"gyrosZ":-3.550428,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.158684,"accelY":-1.05743,"accelZ":0,"gyrosX":24.74324,"gyrosY":-1.072776,"gyrosZ":7.837469,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.167109,"accelY":-1.033316,"accelZ":0,"gyrosX":30.32884,"gyrosY":8.9458,"gyrosZ":4.954036,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.200619,"accelY":-1.004425,"accelZ":0,"gyrosX":33.8128,"gyrosY":11.40155,"gyrosZ":14.52342,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.225175,"accelY":-0.993576,"accelZ":0,"gyrosX":29.89432,"gyrosY":9.685461,"gyrosZ":20.06779,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.264556,"accelY":-0.96064,"accelZ":0,"gyrosX":21.45367,"gyrosY":-2.599771,"gyrosZ":31.44656,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.308822,"accelY":-0.931872,"accelZ":0,"gyrosX":11.72995,"gyrosY":-4.608594,"gyrosZ":27.05902,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.321498,"accelY":-0.902139,"accelZ":0,"gyrosX":-14.92245,"gyrosY":-33.57355,"gyrosZ":3.807594,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.279321,"accelY":-0.85452,"accelZ":0,"gyrosX":-21.20815,"gyrosY":-54.51772,"gyrosZ":-11.81344,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.232951,"accelY":-0.840669,"accelZ":0,"gyrosX":-16.03561,"gyrosY":-37.47482,"gyrosZ":-8.343586,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.221733,"accelY":-0.842619,"accelZ":0,"gyrosX":6.849288,"gyrosY":-22.38449,"gyrosZ":15.76776,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.285804,"accelY":-0.802382,"accelZ":0,"gyrosX":-3.99846,"gyrosY":-18.14645,"gyrosZ":33.3373,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.332924,"accelY":-0.805564,"accelZ":0,"gyrosX":2.304301,"gyrosY":-2.222563,"gyrosZ":31.64819,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.3673,"accelY":-0.846024,"accelZ":0,"gyrosX":0,"gyrosY":-8.26516,"gyrosZ":33.65818,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.383306,"accelY":-0.875298,"accelZ":0,"gyrosX":-2.115099,"gyrosY":4.034795,"gyrosZ":11.57357,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.36287,"accelY":-0.916737,"accelZ":0,"gyrosX":-6.696431,"gyrosY":-7.246841,"gyrosZ":3.969309,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.310918,"accelY":-0.900627,"accelZ":0,"gyrosX":-11.61634,"gyrosY":13.27586,"gyrosZ":-27.56131,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[655527][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.217661,"accelY":-0.630457,"accelZ":0,"gyrosX":-7.901499,"gyrosY":8.57916,"gyrosZ":-19.4129,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[655686][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.152381,"accelY":-0.441338,"accelZ":0,"gyrosX":-5.30111,"gyrosY":5.291467,"gyrosZ":-13.70901,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[655846][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":-0.106685,"accelY":-0.308955,"accelZ":0,"gyrosX":-3.480837,"gyrosY":2.990082,"gyrosZ":-9.716283,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[656004][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":-0.216287,"accelZ":0,"gyrosX":-2.206647,"gyrosY":1.379112,"gyrosZ":-6.921377,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[656156][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":-0.151419,"accelZ":0,"gyrosX":-1.314713,"gyrosY":0,"gyrosZ":-4.964942,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[656316][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":-0.106012,"accelZ":0,"gyrosX":-0.69036,"gyrosY":-0.713945,"gyrosZ":-3.595438,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
[656475][E][Wire.cpp:499] requestFrom(): i2cWriteReadNonStop returned Error -1
Küldve: {"accelX":0,"accelY":0,"accelZ":0,"gyrosX":0,"gyrosY":-1.213706,"gyrosZ":-2.636786,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.339185,"accelZ":0,"gyrosX":13.84215,"gyrosY":-14.20018,"gyrosZ":24.48923,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.551345,"accelZ":0,"gyrosX":42.68128,"gyrosY":-4.938041,"gyrosZ":27.30951,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.675029,"accelZ":0,"gyrosX":49.43966,"gyrosY":3.629427,"gyrosZ":29.23332,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.112354,"accelY":-0.743809,"accelZ":0,"gyrosX":38.26823,"gyrosY":11.53658,"gyrosZ":25.34029,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.204185,"accelY":-0.774377,"accelZ":0,"gyrosX":17.47725,"gyrosY":13.93418,"gyrosZ":18.22738,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.244589,"accelY":-0.82932,"accelZ":0,"gyrosX":3.72508,"gyrosY":-2.007349,"gyrosZ":17.22392,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.294186,"accelY":-0.845661,"accelZ":0,"gyrosX":-11.05411,"gyrosY":-23.77863,"gyrosZ":5.332186,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.287815,"accelY":-0.863325,"accelZ":0,"gyrosX":-14.13542,"gyrosY":-37.88723,"gyrosZ":-16.26073,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.227325,"accelY":-0.849176,"accelZ":0,"gyrosX":-10.01295,"gyrosY":-22.14646,"gyrosZ":-38.98799,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.196335,"accelY":-0.863515,"accelZ":0,"gyrosX":-6.72416,"gyrosY":-11.86532,"gyrosZ":-21.96577,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.208626,"accelY":-0.8622,"accelZ":0,"gyrosX":13.2299,"gyrosY":17.81087,"gyrosZ":5.939098,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.24865,"accelY":-0.826928,"accelZ":0,"gyrosX":27.44049,"gyrosY":36.05137,"gyrosZ":30.30915,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.293806,"accelY":-0.828533,"accelZ":0,"gyrosX":20.08866,"gyrosY":29.18003,"gyrosZ":39.84757,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.306812,"accelY":-0.843718,"accelZ":0,"gyrosX":8.310322,"gyrosY":8.70139,"gyrosZ":46.01149,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.319944,"accelY":-0.873391,"accelZ":0,"gyrosX":-8.577265,"gyrosY":-2.258087,"gyrosZ":30.00409,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.284898,"accelY":-0.936715,"accelZ":0,"gyrosX":-7.423,"gyrosY":-3.531247,"gyrosZ":0.780594,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.280728,"accelY":-0.981409,"accelZ":0,"gyrosX":-13.26998,"gyrosY":-20.79192,"gyrosZ":2.450864,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.263526,"accelY":-0.981859,"accelZ":0,"gyrosX":-18.78271,"gyrosY":-10.86218,"gyrosZ":-3.735672,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.255,"accelY":-1.009274,"accelZ":0,"gyrosX":-18.90422,"gyrosY":-6.948008,"gyrosZ":-8.057087,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.23746,"accelY":-1.008543,"accelZ":0,"gyrosX":-12.93889,"gyrosY":-2.948542,"gyrosZ":-8.814901,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.214562,"accelY":-1.005907,"accelZ":0,"gyrosX":-8.291405,"gyrosY":-2.013039,"gyrosZ":-5.475142,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.204026,"accelY":-1.001278,"accelZ":0,"gyrosX":-5.372517,"gyrosY":-0.721545,"gyrosZ":-3.40754,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.19775,"accelY":-0.994376,"accelZ":0,"gyrosX":-3.75525,"gyrosY":-1.241927,"gyrosZ":-2.697623,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.197239,"accelY":-1.015912,"accelZ":0,"gyrosX":3.958517,"gyrosY":-4.780241,"gyrosZ":-3.547246,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.193511,"accelY":-1.023004,"accelZ":0,"gyrosX":16.33831,"gyrosY":-1.893701,"gyrosZ":2.709926,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.196102,"accelY":-1.006728,"accelZ":0,"gyrosX":29.48355,"gyrosY":12.09482,"gyrosZ":7.841091,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.250431,"accelY":-1.036203,"accelZ":0,"gyrosX":34.06842,"gyrosY":10.12953,"gyrosZ":26.16268,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.248178,"accelY":-0.986744,"accelZ":0,"gyrosX":38.66562,"gyrosY":19.65001,"gyrosZ":16.86565,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.277655,"accelY":-0.941648,"accelZ":0,"gyrosX":24.92794,"gyrosY":-4.711611,"gyrosZ":19.34399,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.325682,"accelY":-0.920555,"accelZ":0,"gyrosX":8.331402,"gyrosY":-17.78917,"gyrosZ":7.072723,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.318505,"accelY":-0.869022,"accelZ":0,"gyrosX":-12.98693,"gyrosY":-38.64575,"gyrosZ":-14.99655,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.284843,"accelY":-0.881656,"accelZ":0,"gyrosX":-13.84412,"gyrosY":-26.21788,"gyrosZ":-35.12596,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.268457,"accelY":-0.859737,"accelZ":0,"gyrosX":-2.526592,"gyrosY":4.525906,"gyrosZ":-15.3051,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.252447,"accelY":-0.871713,"accelZ":0,"gyrosX":10.59873,"gyrosY":19.11679,"gyrosZ":16.65195,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.302396,"accelY":-0.847504,"accelZ":0,"gyrosX":7.882638,"gyrosY":18.97009,"gyrosZ":43.44631,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.335823,"accelY":-0.837516,"accelZ":0,"gyrosX":-14.31786,"gyrosY":20.22772,"gyrosZ":43.64359,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.350725,"accelY":-0.893732,"accelZ":0,"gyrosX":-30.92997,"gyrosY":24.12179,"gyrosZ":18.35266,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.339404,"accelY":-0.957106,"accelZ":0,"gyrosX":-25.75386,"gyrosY":5.97131,"gyrosZ":4.368108,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.305186,"accelY":-0.974955,"accelZ":0,"gyrosX":-15.26952,"gyrosY":3.287346,"gyrosZ":-8.503526,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.25882,"accelY":-0.977122,"accelZ":0,"gyrosX":-9.345748,"gyrosY":-4.348681,"gyrosZ":-8.431225,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.251413,"accelY":-1.000904,"accelZ":0,"gyrosX":-8.60216,"gyrosY":0.712208,"gyrosZ":-18.52565,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.220154,"accelY":-1.00576,"accelZ":0,"gyrosX":-8.947298,"gyrosY":1.076204,"gyrosZ":-14.2513,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.186188,"accelY":-1.002347,"accelZ":0,"gyrosX":-6.019429,"gyrosY":0,"gyrosZ":-11.8226,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.169883,"accelY":-1.007063,"accelZ":0,"gyrosX":-5.829462,"gyrosY":1.163918,"gyrosZ":-9.737785,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.15964,"accelY":-1.000476,"accelZ":0,"gyrosX":-4.560607,"gyrosY":2.345073,"gyrosZ":-7.142535,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.127935,"accelY":-1.00861,"accelZ":0,"gyrosX":-2.037295,"gyrosY":9.171881,"gyrosZ":-4.084639,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.108158,"accelY":-0.995407,"accelZ":0,"gyrosX":1.542764,"gyrosY":26.40408,"gyrosZ":-2.305943,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.113723,"accelY":-0.979133,"accelZ":0,"gyrosX":6.054913,"gyrosY":29.87426,"gyrosZ":-1.509712,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.141642,"accelY":-0.98239,"accelZ":0,"gyrosX":-1.462919,"gyrosY":37.93239,"gyrosZ":10.5759,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.174662,"accelY":-0.988186,"accelZ":0,"gyrosX":-13.77884,"gyrosY":36.70743,"gyrosZ":2.973224,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.142845,"accelY":-0.995246,"accelZ":0,"gyrosX":-22.21219,"gyrosY":31.82858,"gyrosZ":-7.913531,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":-0.11984,"accelY":-1.001213,"accelZ":0,"gyrosX":-19.81173,"gyrosY":31.16607,"gyrosZ":-6.827389,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.989936,"accelZ":0,"gyrosX":-18.18178,"gyrosY":30.44581,"gyrosZ":-6.909839,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.98922,"accelZ":0,"gyrosX":-14.71868,"gyrosY":30.65156,"gyrosZ":-7.466789,"trainingId":14203}
🔹 API válasz:
{"userId":"47f44e1b-0127-4e01-a5b9-b9c3a959560a","start":"2025-11-20T15:51:45.878","trainingId":14203}
Küldve: {"accelX":0,"accelY":-0.97949,"accelZ":0,"gyrosX":-12.79833,"gyrosY":37.07497,"gyrosZ":-7.568105,"trainingId":14203}
[Server]: stop
>> Leáll az adatküldés

"""

# 1) JSON-ek kiszedése a "Küldve: {...}" sorokból
matches = re.findall(r'Küldve:\s*({.*?})', raw)
data = [json.loads(m) for m in matches]

if not data:
    raise ValueError("Nem találtam 'Küldve: {...}' sorokat a raw szövegben.")

# 2) Időtengely (mint index)
t = list(range(len(data)))

# 3) Adatsorok szétbontása
accelX = [d["accelX"] for d in data]
accelY = [d["accelY"] for d in data]
accelZ = [d["accelZ"] for d in data]

gyrosX = [d["gyrosX"] for d in data]
gyrosY = [d["gyrosY"] for d in data]
gyrosZ = [d["gyrosZ"] for d in data]

# 4) Accelerometer grafikon
plt.figure(figsize=(12, 6))
plt.plot(t, accelX, label="accelX")
plt.plot(t, accelY, label="accelY")
plt.plot(t, accelZ, label="accelZ")
plt.xlabel("Mint index")
plt.ylabel("Gyorsulás (g egység)")
plt.title("Accelerometer adatok")
plt.grid(True)
plt.legend()

# 5) Giroszkóp grafikon
plt.figure(figsize=(12, 6))
plt.plot(t, gyrosX, label="gyrosX")
plt.plot(t, gyrosY, label="gyrosY")
plt.plot(t, gyrosZ, label="gyrosZ")
plt.xlabel("Mint index")
plt.ylabel("Szögsebesség (°/s)")
plt.title("Giroszkóp adatok")
plt.grid(True)
plt.legend()

plt.show()
