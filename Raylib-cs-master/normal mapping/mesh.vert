#version 330

// Input vertex attributes
in vec3 vertexPosition;
in vec2 vertexTexCoord;
in vec3 vertexNormal;
in vec3 vertexTangent;

//Frag
out vec3 fragPos;
out vec3 fragNormal;
out vec2 fragTexCoord;
out vec4 fragPosLightSpace;
//Tangent
out vec3 tangentLightPos;
out vec3 tangentViewPos;
out vec3 tangentFragPos;

uniform mat3 matNormal;
uniform mat4 matProjection;
uniform mat4 matView;
uniform mat4 matModel;
uniform mat4 matLight;
uniform mat4 mvp;

uniform vec3 lightPos;
uniform vec3 viewPos;


void main()
{
	fragPos = vec3(matModel * vec4(vertexPosition, 1.0));
    fragTexCoord = vertexTexCoord;

    fragNormal = transpose(inverse(mat3(matModel))) * vertexNormal;
    fragPosLightSpace = matLight * vec4(fragPos, 1.0);

    mat3 normalMatrix = transpose(inverse(mat3(matModel)));
    vec3 T = normalize(normalMatrix * vertexTangent);
    vec3 N = normalize(normalMatrix * vertexNormal);
    T = normalize(T - dot(T, N) * N);
    vec3 B = cross(N,T);

    mat3 TBN = transpose(mat3(T,B,N));
    tangentLightPos = TBN * lightPos;
    tangentViewPos = TBN * viewPos;
    tangentFragPos = TBN * fragPos;

    //gl_Position = matProjection * matView * vec4(fragPos, 1.0);
    gl_Position = mvp * vec4(vertexPosition, 1.0);
}