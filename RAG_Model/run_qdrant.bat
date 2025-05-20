@echo off
echo Starting Qdrant container...
docker start QdrantVectorDb

if %ERRORLEVEL% NEQ 0 (
    echo Error: Docker failed to start Qdrant.
) else (
    echo Qdrant is running on port 6333.
)
pause
