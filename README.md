# 패스트캠퍼스 현실 세상의 TDD 실전편

폴더와 파일은 프로젝트 루트 경로를 기준으로 한다.

## 도커 컴포즈를 이용한 DB 실행

### 환경변수 설정

- LOCAL_DEVDB_SUPER_PASSWORD
- LOCAL_DEVDB_PASSWORD = "mysecret-pp#" (바로 실행할 수 있도록 기본으로 설정되어 있는 값)

### docker compose 실행

```bash
cd db
chmod 744 init.sh
docker compose up -d
```

### 기타

- 관리자 계정 : shopadmin
- 도커 이미지에서 관리자 계정의 이름으로 디비를 생성하며, 이름을 지정하려면 `environment`에 `POSTGRES_DB` 환경변수 값을 추가한다.

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
cd Orders/Orders.Api
dotnet ef database update -p ../Orders.Infrastructure/Orders.Infrastructure.csproj

# SellersDB Migration
cd Sellers/Sellers.Api
dotnet ef database update
```

### 테스트 환경 연결 정보 설정

- Orders/Orders.UnitTests/OrdersServer.cs - "string ConnectionString"
- Sellers/Selleres.Testing/SellersServer.cs - "string ConnectionString"

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
