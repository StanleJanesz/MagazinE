pipeline {
    agent any

    environment {
        DOCKER_COMPOSE_FILE = "docker-compose.yml"
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build Docker Images') {
            steps {
                echo "building docker images..."                
                powershell "docker-compose -f ${DOCKER_COMPOSE_FILE} build"
            }
        }

        stage('Start Containers') {
            steps {
                echo "starting containers..."
                powershell "docker-compose -f ${DOCKER_COMPOSE_FILE} up -d"
                echo "⌛ Czekam na MSSQL (20s)..."
                powershell "sleep 20"
            }
        }

        stage('Run Backend Tests') {
            steps {
                echo "running backend tests..."
                powershell "docker-compose exec -T magazineapi dotnet test --no-build --logger:trx"
            }
        }

        stage('Run Frontend Tests') {
            steps {
                echo "running frontend tests..."
                powershell "docker-compose exec -T frontend npm test -- --watchAll=false"
            }
        }

        stage('Teardown') {
            steps {
                echo "Teardown..."
                powershell "docker-compose -f ${DOCKER_COMPOSE_FILE} down -v"
            }
        }
    }

    post {
        always {
            echo "pipeline ended. Final cleaning..."
            sh "docker-compose down -v || true"
        }
    }
}
