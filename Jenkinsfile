pipeline {
    agent any
	options {
        ansiColor('xterm')
    }

    stages {
        stage('Checkout') {
            steps {
                git branch: env.BRANCH_NAME, credentialsId: 'GitHub PAT', url: 'https://github.com/nullinside-development-group/nullinside-cicd-github.git'
            }
        }
        
        stage('Build & Deploy') {
            steps {
				withCredentials([
					usernamePassword(credentialsId: 'GitHub PAT', passwordVariable: 'GITHUB_PAT', usernameVariable: 'GITHUB_USERNAME'),
				]) {
					sh """
						bash go.sh 
					"""
				}
            }
        }
    }
	
	post {
		always {
			cleanWs cleanWhenFailure: false, notFailBuild: true
		}
	}
}
