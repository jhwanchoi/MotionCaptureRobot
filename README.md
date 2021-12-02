# MotionCaptureRobot_DATASET

## Explain about this DATASET
git에 올려진 코드와 데이터셋은 사람의 제스처에 따라 로봇을 제어하기 위해 만든 자료입니다.
로보티즈사에서 제공하는 '로보티즈 엔지니어 KIT1' 과 마이크로 소프트 사에서 제공하는 'KINECT V2 CAMERA'를 이용하여 딥러닝 모델에 적용될 데이터를 제작하였습니다.

본 데이터 셋은 KINECT V2의 11 관절 정보값(Head, Neck, SpineShoulder, SpineMid, SpineBase, ShoulderRight, ShoulderLeft, ElbowLeft, ElbowRight, WristLeft, WristRight)의 X,Y,Z와 로보티즈 엔지니어 KIT1의 모터 12개 위치정보값으로 구성되어 있습니다.

이를 통해 입력 33, 출력 12의 딥러닝 모델을 만들어 학습 시켰으며, 학습된 모델이 꽤 괜찮은 PREDICT를 주는 것을 확인 할 수 있었습니다.
해당 딥러닝 코드는 Google Colab을 사용하였음을 알려드립니다.


## The way of collection using our Codes
데이터를 만드는 것은 3사람이 진행할때 가장 적합하며, 전체적인 과정은 아래와같습니다. 
A가 모션을 취하면 kinect V2로 관절값을 체크하고, B는 A의 모션에 따라 같은 움직임으로 로봇을 움직여 관절 정보를 입력받게 합니다. C는 A,B가 만들어주는 데이터를 버튼을 클릭하여 데이터를 생성합니다.
마이크로 소프트사에서 제공하는 Kinect V2 SDK와 로보티즈사에서 제공하는 SDK를 수정하여 작성된 코드입니다.   
https://www.microsoft.com/en-us/download/details.aspx?id=44561   
https://github.com/ROBOTIS-GIT

1. Kinect V2을 제어할 데스크 탑에 연결하고 로보티즈 엔지니어 Kit1에 전원을 인가합니다.
2. DATASET\DATA_SET\server\c#\protocol2.0\motor_control_with_window\win64\motor_control_with_window.sln 을 키고 CTRL+F5실행합니다.
3. DATASET\DATA_SET\BodyBasics-WPF 을 키고 CTRL+F5실행합니다.
(motor_control_with_window - 로보티즈 엔지니어 Kit1 / BodyBasics - Kinect V2)
4. motor_control_with_window에서 [포트연결 버튼 클릭] -> [서버열기] -> [서비스시작] -> [토크해제] (만약 연결이 안된다면 코드내에 포트번호 확인 COM X 확인하여 수정)
5. BodyBasics-WPF에서 화면에 관절 포착 확인시 [포트연결 버튼 클릭]
6. A,B 사람이 각각 모션데이터를 만들어주면, C가 motor_control_with_window의 [DATA_SET 버튼 클릭] 하여 데이터를 만든다.
7. 만들어진 데이터셋은 DATASET\DATA_SET\server\c#\protocol2.0\motor_control_with_window\win64\motor_control_with_window\bin\Debug\SaveMt에 sample.txt로 저장된다
