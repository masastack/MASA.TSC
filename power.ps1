param($t,$u,$p)

#docker build 
#Write-Host "Hello.$args"
Write-Host "Hello $t"

docker login --username=$u registry.cn-hangzhou.aliyuncs.com --password=$p

$ServiceDockerfilePath="./src/Services/Masa.Tsc.Service.Admin/Dockerfile"
$ServiceServerName="masa-tsc-service-admin"
$WebDockerfilePath="./src/Web/Masa.Tsc.Web.Admin.Server/Dockerfile"
$WebServerName="masa-tsc-web-admin"

docker build -t registry.cn-hangzhou.aliyuncs.com/masastack/${ServiceServerName}:$t  -f $ServiceDockerfilePath .
docker push registry.cn-hangzhou.aliyuncs.com/masastack/${ServiceServerName}:$t 

docker build -t registry.cn-hangzhou.aliyuncs.com/masastack/${WebServerName}:$t  -f $WebDockerfilePath .
docker push registry.cn-hangzhou.aliyuncs.com/masastack/${WebServerName}:$t 