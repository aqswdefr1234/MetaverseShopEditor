# MetaverseShopEditor

이 프로젝트는 Unity로 제작되었으며 Windows어플리케이션 전용입니다.
5가지 주요 기능을 가지고 있습니다.

1. 이미지만을 이용하여 T-shirts 오브젝트 제작하기 (베타)
2. .obj파일 unity GameObject로 변환하여 로드하기
3. 로드한 obj파일의 color 변경 및 이미지 삽입하기
4. 만들어진 오브젝트 파일을 Room에 로드 후 position, rotation, scale 변경가능
5. 최종적으로 로드된 오브젝트들과 Room 정보를 저장.

이미지만을 이용하여 티셔츠3D 오브젝트를 제작할 수 있습니다. 사용자는 이미지 위에 있는 정점 오브젝트를 움직여 원하는 티셔츠 mesh와 UV값을 얻을 수 있습니다.
이 오브젝트의 지오메트릭 데이터를 파싱하여 json으로 저장합니다.

그리고 unity 에서 빌드 된 프로그램에서는 지원되지 않는 obj converter 기능이 있습니다. obj파일을 GameObject로 변환합니다.
그 후 obj파일 내부의 여러 오브젝트들을 분리하여 개별로 color값과 이미지를 삽입할 수 있습니다.
로드된 obj 파일의 지오메트릭 데이터를 파싱하여 json으로 저장합니다.
이 때 json파일에는 이미지 데이터가 텍스트 값으로 변환되어 저장됩니다.

이렇게 파싱된 데이터를 가지고 Room 안에 배치할 수 있습니다. 이 오브젝트들의 position, rotation, scale 값들은 수치를 직접 변경할 수 있으며,
position값은 xyz축을 마우스로 드래그하여 오브젝트들을 옮길수도 있습니다.

Room은 현재(23/05/19) 2가지가 존재하며 각각 서로 다른 라이트 맵을 가지고 있습니다. 이 라이트맵들은 스크립트에서 직접 할당하여,
Room 별로 서로다른 baked 라이트맵을 로드합니다.

최종적으로 저장버튼을 누르면 현재 Room정보와 로드된 모든 오브젝트들의 정보를 json으로 저장합니다.
이미지 파일또한 string값으로 변환하여 json파일에 포함시켰기 때문에 데이터 파일의 크기를 최소화 합니다.

* 이 프로젝트는 사용자의 로컬폴더에 직접 접근합니다.
