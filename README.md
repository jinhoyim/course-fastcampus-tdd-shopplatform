# 현실 세상의 TDD 실전편 실습 코드
The RED : 현실 세상의 TDD 실전편 : 설계 확장성을 위한 코드 개선 방법
https://fastcampus.co.kr/dev_red_ygw2

강의 앞 부분의 기존 코드는 프로젝트 구조가 간단하고 뒷 부분의 코드는 계층을 나눠간다.
개인적으로 경험한 프로젝트들은 어떻게든 구조가 나눠져있어서 마음대로 나누고 실습했다.
또 강의의 코드와 GitHub에 공유된 소스코드가 다르며, 이 실습 코드도 조금 다르다.
사용해보지 않은 방법은 그대로 따라해보고, 입맛에 맞게 바꿔보거나 먼저 작성해보기도 했다.

## 실습 환경 구성
폴더와 파일은 프로젝트 루트 경로를 기준으로 한다.
도커 컴포즈를 이용한 DB 를 준비한다.

### docker compose 실행
```bash
cd db
docker compose up -d
```

### DB 마이그레이션 실행

- 테스트 DB 는 테스트 프로젝트에서 마이그레이션 된다.

### dotnet ef 도구 설치

```bash
dotnet tool install --global dotnet-ef
```

### 개발 서버 연결 정보 설정

- Orders/Orders.Api/appsettings.Development.json
- Sellers/Sellers.Api/appsettings.Development.json
- accounting/api/src/main/resources/application.yml

### 개발 서버 마이그레이션 실행

```bash
# OrderingDB Migration
dotnet ef database update -p Orders/Orders.Infrastructure -s Orders/Orders.Api

# SellersDB Migration
dotnet ef database update -p Sellers/Sellers.Sql -s Sellers/Sellers.Api
```

### 테스트 환경 연결 정보 설정

- Orders/Orders.UnitTests/OrdersServer.cs
- Sellers/Sellers.Testing/SellersServer.cs
- Sellers/Sellers.UnitTests/api/shops/id/Get_specs.cs

## 개발 서버 실행

```bash
# 주문 API 실행
dotnet run --project ./Orders/Orders.Api

# 판매자 API 실행
dotnet run --project ./Sellers/Sellers.Api

# 정산 API 실행
./gradlew :accounting:api:bootRun
```

- [주문 API](http://localhost:5187/swagger/index.html)
- [판매자 API](http://localhost:5279/swagger/index.html)
- [정산 API](http://localhost:8091/swagger-ui/index.html)

## 테스트 실행

```bash
# Orders.UnitTests, Sellers.UnitTests 테스트 실행
dotnet test

# accounting.unittest 테스트 실행
./gradlew :accounting:unittest:test
```
