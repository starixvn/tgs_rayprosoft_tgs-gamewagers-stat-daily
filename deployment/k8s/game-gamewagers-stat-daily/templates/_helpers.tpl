{{/*
Expand the name of the chart.
*/}}
{{- define "game-gamewagers-stat-daily.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "game-gamewagers-stat-daily.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "game-gamewagers-stat-daily.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "game-gamewagers-stat-daily.labels" -}}
helm.sh/chart: {{ include "game-gamewagers-stat-daily.chart" . }}
{{ include "game-gamewagers-stat-daily.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "game-gamewagers-stat-daily.selectorLabels" -}}
app.kubernetes.io/name: {{ include "game-gamewagers-stat-daily.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
app: {{ include "game-gamewagers-stat-daily.name" . }}
tgs.kind: game
tgs.game.kind: gamewagers
tgs.gamewagers.kind: stats
tgs.node.selector.kind: gamewagers
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "game-gamewagers-stat-daily.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "game-gamewagers-stat-daily.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}
