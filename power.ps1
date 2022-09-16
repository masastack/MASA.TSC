param($t,$u,$p,$s)

#docker build 
#Write-Host "Hello.$args"
Write-Host "Hello $t"

switch($s)
{
  service {$DockerfilePath="./src/Services/Masa.Tsc.Service.Admin/Dockerfile";$ServerName="masa-tsc-service-admin"}
  web  {$DockerfilePath="./src/Web/Masa.Tsc.Web.Admin.Server/Dockerfile";$ServerName="masa-tsc-web-admin"}
}
docker login --username=$u registry.cn-hangzhou.aliyuncs.com --password=$p
docker build -t registry.cn-hangzhou.aliyuncs.com/masa/${ServerName}:$t  -f $DockerfilePath .
docker push registry.cn-hangzhou.aliyuncs.com/masa/${ServerName}:$t 