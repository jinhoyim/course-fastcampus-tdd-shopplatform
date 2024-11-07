# 패스트캠퍼스 현실 세상의 TDD 실전편

폴더와 파일은 프로젝트 루트 경로를 기준으로 한다.

## 도커 컴포즈를 이용한 DB 실행

### docker compose 실행
```bash
cd db
docker compose up -d
```

## DB 마이그레이션 실행

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
