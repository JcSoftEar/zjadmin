pipeline {
    agent any

    environment {
        IMAGE_NAME = 'zjadmin'
        IMAGE_TAG = "${env.BUILD_NUMBER}"
        CONTAINER_NAME = 'zjadmin'
        HOST_PORT = '417'
    }

    stages {
        stage('Checkout') {
            steps {
                git branch: 'master',
                    url: 'https://github.com/JcSoftEar/zjadmin.git',
                    credentialsId: 'github-token'
            }
        }

        stage('Build & Package') {
            steps {
                sh """
                    cd ${WORKSPACE}
                    tar cz --exclude=node_modules --exclude=.git \\
                        -f - . | docker build -f Dockerfile -t ${IMAGE_NAME}:${IMAGE_TAG} -
                """
            }
        }

        stage('Deploy') {
            steps {
                sh """
                    docker stop ${CONTAINER_NAME} || true
                    docker rm ${CONTAINER_NAME} || true
                    docker run -d \\
                        --name ${CONTAINER_NAME} \\
                        --restart unless-stopped \\
                        -p ${HOST_PORT}:80 \\
                        -v zjadmin-data:/app/Data \\
                        -v zjadmin-logs:/app/logs \\
                        -e ASPNETCORE_ENVIRONMENT=Production \\
                        -e ASPNETCORE_URLS=http://+:5000 \\
                        -e "ConnectionStrings__DefaultConnection=Data Source=Data/ZJAdmin.db" \\
                        ${IMAGE_NAME}:${IMAGE_TAG}
                """
            }
        }

        stage('Cleanup') {
            steps {
                sh "docker image prune -f --filter 'dangling=true'"
            }
        }
    }

    post {
        failure {
            echo 'Deploy failed!'
        }
    }
}
