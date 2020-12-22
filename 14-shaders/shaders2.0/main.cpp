#include "ShaderUtility.h"

int mode;
float factor;

std::string tex_Path_1 = "";
std::string tex_Path_2 = "";

ShaderUtils Shader;

GLuint VBO_vertex;
GLuint VBO_color;
GLuint VBO_texture;
GLuint VBO_normal;
GLuint texture_1;
GLuint texture_2;

const char* vsSource = "#version 330 core\n"
"layout(location = 0) in vec3 coord;\n"//координаты вершины
"layout(location = 1) in vec2 vertexUV;\n"//текстурные координаты
"layout(location = 2) in vec3 vertexColor;\n"//матрица цветов дл€ mod2
"uniform mat4 MVP;\n"//матрица преобразований
"out vec2 UV;\n"
"out vec3 fragmentColor;\n"
"void main() {\n"
"	gl_Position = MVP * vec4(coord, 1.0);\n"//позици€ вершины 
"	fragmentColor = vertexColor;\n"
"	UV = vertexUV;\n"//текстурна€ координата вершины
"}\n";
//mono texture
const char* fsSource_1 = "#version 330 core\n"
"in vec2 UV;\n"
"uniform sampler2D myTextureSampler;\n"
"void main() {\n"
"	gl_FragColor = texture(myTextureSampler, UV);\n"
"}\n";
//texture + color
const char* fsSource_2 = "#version 330 core\n"
"in vec2 UV;\n"
"uniform sampler2D myTextureSampler;\n"
"in vec3 fragmentColor;\n"
"void main() {\n"
"	gl_FragColor = texture(myTextureSampler, UV) * vec4(fragmentColor, 1.0);\n"
"}\n";
//textures mix
const char* fsSource_3 = "#version 330 core\n"
"in vec2 UV;\n"
"uniform sampler2D myTextureSampler;\n"
"uniform sampler2D myTextureSampler1;\n"
"in vec3 fragmentColor;\n"
"uniform float mix_f;\n"
"void main() {\n"
"	gl_FragColor = mix(texture(myTextureSampler, UV), texture(myTextureSampler1, UV), mix_f);\n"
"}\n";

const char* vsSource_2 = "#version 330 core\n"
"#define VERT_POSITION 0\n"
"#define VERT_TEXCOORD 1\n"
"#define VERT_NORMAL 2\n"
"layout(location = VERT_POSITION) in vec3 position;\n"
"layout(location = VERT_TEXCOORD) in vec2 texcoord;\n"
"layout(location = VERT_NORMAL) in vec3 normal;\n"
"uniform struct Transform {\n"
"	mat4 model;\n"
"	mat4 viewProjection;\n"
"	mat3 normal;\n"
"	vec3 viewPosition;\n"
"} transform;\n"
"uniform struct PointLight{\n"//источник света
"	vec4 position;\n"
"	vec4 ambient;\n"
"	vec4 diffuse;\n"
"	vec4 specular;\n"
"	vec3 attenuation;\n"
"} light;\n"
"out struct Vertex {\n"//передаЄм параметры вершины в структуру и в фрагментный шейдер
"	vec2 texcoord;\n"
"	vec3 normal;\n"
"	vec3 lightDir;\n"
"	vec3 viewDir;\n"
"	float distance;\n"
"} Vert;\n"
"void main(void){\n"
"	vec4 vertex = transform.model * vec4(position, 1.0);\n"//позици€ вершины с учЄтом поворотов и точки зрени€
"	vec4 lightDir = light.position - vertex;\n"//вектор от вершины на источник света
"	gl_Position = transform.viewProjection * vertex;\n"//позици€ вершины дл€ отрисовки
"	Vert.texcoord = texcoord;\n"//передаЄм текстурную координату вершины в Vert
"	Vert.normal = transform.normal * normal;\n"//преобразованна€ нормаль
"	Vert.lightDir = vec3(lightDir);\n"//передаЄм вектор на источник света в Vert
"	Vert.viewDir = transform.viewPosition - vec3(vertex);\n"//вектор от вершины на зрител€
"	Vert.distance = length(lightDir);\n"//длина вектора на источник света дл€ нормализации
"}\n";
//per pixel lighting
const char* fsSource_4 = "#version 330 core\n"
"#define FRAG_OUTPUT0 0\n"
"layout(location = FRAG_OUTPUT0) out vec4 color;\n"
"uniform struct PointLight{\n"
"	vec4 position;\n"
"	vec4 ambient;\n"
"	vec4 diffuse;\n"
"	vec4 specular;\n"
"	vec3 attenuation;\n"
"} light;\n"
"uniform struct Material {\n"
"	sampler2D texture;\n"
"	vec4 ambient;\n"
"	vec4 diffuse;\n"
"	vec4 specular;\n"
"	vec4 emission;\n"
"	float shininess;\n"
"} material;\n"
"in struct Vertex {\n"
"	vec2 texcoord;\n"
"	vec3 normal;\n"
"	vec3 lightDir;\n"
"	vec3 viewDir;\n"
"	float distance;\n"
"} Vert;\n"
"void main(void){\n"
"	vec3 normal = normalize(Vert.normal);\n"
"	vec3 lightDir = normalize(Vert.lightDir);\n"
"	vec3 viewDir = normalize(Vert.viewDir);\n"
	//значение затухани€
"	float attenuation = 1.0 / (light.attenuation[0] + light.attenuation[1] * Vert.distance + light.attenuation[2] * Vert.distance * Vert.distance);\n"
"	color = material.emission;\n"//собственное свечение объекта
"	color += material.ambient * light.ambient * attenuation;\n"//внешний свет с учЄтом затухани€
"	float Ndot = max(dot(normal, lightDir), 0.0);\n"//если 0, то луч не попадает на вершину(>90 градусов)
"	color += material.diffuse * light.diffuse * Ndot * attenuation;\n"//диффузный свет с затуханием
"	vec3 H = normalize(lightDir + viewDir);\n"
"	float RdotVpow = max(pow(dot(normal, H), material.shininess), 0.0);\n"
"	color += material.specular * light.specular * RdotVpow * attenuation;\n"//блики
"	color *= texture(material.texture, Vert.texcoord);\n"//добавл€ем цвет текстуры
"}\n";
//toon shading
const char* fsSource_5 = "#version 330 core\n"
"#define FRAG_OUTPUT0 0\n"
"layout(location = FRAG_OUTPUT0) out vec4 color;\n"
"uniform struct PointLight{\n"
"	vec4 position;\n"
"	vec4 ambient;\n"
"	vec4 diffuse;\n"
"	vec4 specular;\n"
"	vec3 attenuation;\n"
"} light;\n"
"uniform struct Material {\n"
"	sampler2D texture;\n"
"	vec4 ambient;\n"
"	vec4 diffuse;\n"
"	vec4 specular;\n"
"	vec4 emission;\n"
"	float shininess;\n"
"} material;\n"
"in struct Vertex {\n"
"	vec2 texcoord;\n"
"	vec3 normal;\n"
"	vec3 lightDir;\n"
"	vec3 viewDir;\n"
"	float distance;\n"
"} Vert;\n"
"void main(void){\n"
"	vec3 normal = normalize(Vert.normal);\n"
"	vec3 lightDir = normalize(Vert.lightDir);\n"
"	float diff = 0.2 + max(dot(normal, lightDir), 0.0);\n"//провер€ем угол между нормалью и вектором на свет
"	if(diff < 0.4)\n"
"		color = material.diffuse * 0.3;\n"
"	else if(diff < 0.7)\n"
"		color = material.diffuse;\n"//затен€ем в зависимости от угла падени€ луча
"	else\n"
"		color = material.diffuse * 1.3;\n"
"}\n";
//Bidirectional lighting model
const char* fsSource_6 = "#version 330 core\n"
"#define FRAG_OUTPUT0 0\n"
"layout(location = FRAG_OUTPUT0) out vec4 color;\n"
"uniform struct PointLight{\n"
"	vec4 position;\n"
"	vec4 ambient;\n"
"	vec4 diffuse;\n"
"	vec4 specular;\n"
"	vec3 attenuation;\n"
"} light;\n"
"uniform struct Material {\n"
"	sampler2D texture;\n"
"	vec4 ambient;\n"
"	vec4 diffuse;\n"
"	vec4 specular;\n"
"	vec4 emission;\n"
"	float shininess;\n"
"} material;\n"
"in struct Vertex {\n"
"	vec2 texcoord;\n"
"	vec3 normal;\n"
"	vec3 lightDir;\n"
"	vec3 viewDir;\n"
"	float distance;\n"
"} Vert;\n"
"void main(void){\n"
"vec4 color0 = light.diffuse;\n"
"vec4 color2 = vec4(1.0 - light.diffuse.r, 1.0 - light.diffuse.g, 1.0 - light.diffuse.b, 1);\n"//инвертируем
"vec3 n2 = normalize(Vert.normal);\n"
"vec3 l2 = normalize(Vert.lightDir);\n"
"vec4 diff = color0 * max(dot(n2, l2), 0.0) + color2 * max(dot(n2, -l2), 0.0);\n"//проводим интерпол€цию между двум€ цветами
"color = diff * texture(material.texture, Vert.texcoord);\n"//добавл€ем цвет текстуры
"}\n";

double rotate_xx = 0;
double rotate_yy = 0;
double rotate_zz = 0;



void checkOpenGLerror()
{
	GLenum errCode;

	if ((errCode = glGetError()) != GL_NO_ERROR)
	{
		std::cout << "OpenGl error! - " << gluErrorString(errCode);
	}
}

void Load_Textures()
{
	texture_2 = SOIL_load_OGL_texture(tex_Path_1.c_str(), SOIL_LOAD_AUTO, SOIL_CREATE_NEW_ID, SOIL_FLAG_MIPMAPS | SOIL_FLAG_INVERT_Y | SOIL_FLAG_NTSC_SAFE_RGB | SOIL_FLAG_COMPRESS_TO_DXT);
	texture_1 = SOIL_load_OGL_texture(tex_Path_2.c_str(), SOIL_LOAD_AUTO, SOIL_CREATE_NEW_ID, SOIL_FLAG_MIPMAPS | SOIL_FLAG_INVERT_Y | SOIL_FLAG_NTSC_SAFE_RGB | SOIL_FLAG_COMPRESS_TO_DXT);
}

void Init_Shader()
{
	if (mode == 0)// mono texture
	{
		Shader.load(vsSource, fsSource_1);
	}
	else if (mode == 1)//texture + color mix
	{
		Shader.load(vsSource, fsSource_2);
	}
	else if (mode == 2)// textures mix
	{
		Shader.load(vsSource, fsSource_3);
	}
	else if (mode == 3)// per pixel lighting
	{
		Shader.load(vsSource_2, fsSource_4);
	}
	else if (mode == 4)//toon shading
	{
		Shader.load(vsSource_2, fsSource_5);
	}
	else if (mode == 5)
	{
		Shader.load(vsSource_2, fsSource_6);
	}

	int link_ok;
	glGetProgramiv(Shader.ShaderProgram, GL_LINK_STATUS, &link_ok);

	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}

	checkOpenGLerror();
}

void initVBO(Mesh mesh)
{
	int vert_size = 0;
	int col_size = 0;
	GLfloat* vertices = triangulate(mesh.pointList, mesh.pointList.size(), mesh.indicesList, mesh.indicesList.size(), vert_size);
	
	vector<Point3D> old_colors;
	for (size_t i = 0; i < mesh.pointList.size(); i++)
	{
		double r = (rand() % 100) * 0.01;
		double g = (rand() % 100) * 0.01;
		double b = (rand() % 100) * 0.01;
		old_colors.push_back(Point3D(r, g, b));
	}

	int old_col_size = old_colors.size() * 3;

	GLfloat* colors = triangulate(old_colors, old_col_size, mesh.indicesList, mesh.indicesList.size(), col_size);

	GLfloat* uvs = new GLfloat[mesh.texturePoint.size() * 2];
	int ind = 0;
	for (size_t i = 0; i < mesh.texturePoint.size(); i++)
	{
		uvs[ind] = mesh.texturePoint[i].x;
		ind++;
		uvs[ind] = mesh.texturePoint[i].y;
		ind++;
	}
	int uv_size = ind;
	
	GLfloat* normals = new GLfloat[mesh.normalList.size() * 3];
	ind = 0;
	for (size_t i = 0; i < mesh.normalList.size(); i++)
	{
		normals[ind] = mesh.normalList[i].x;
		ind++;
		normals[ind] = mesh.normalList[i].y;
		ind++;
		normals[ind] = mesh.normalList[i].z;
		ind++;
	}
	int norm_size = ind;

	glGenBuffers(1, &VBO_vertex);
	glBindBuffer(GL_ARRAY_BUFFER, VBO_vertex);
	glBufferData(GL_ARRAY_BUFFER, vert_size * 4, vertices, GL_STATIC_DRAW);

	glGenBuffers(1, &VBO_color);
	glBindBuffer(GL_ARRAY_BUFFER, VBO_color);
	glBufferData(GL_ARRAY_BUFFER, col_size * 4, colors, GL_STATIC_DRAW);

	glGenBuffers(1, &VBO_texture);
	glBindBuffer(GL_ARRAY_BUFFER, VBO_texture);
	glBufferData(GL_ARRAY_BUFFER, uv_size * 4, uvs, GL_STATIC_DRAW);

	glGenBuffers(1, &VBO_normal);
	glBindBuffer(GL_ARRAY_BUFFER, VBO_normal);
	glBufferData(GL_ARRAY_BUFFER, norm_size * 4, normals, GL_STATIC_DRAW);

	checkOpenGLerror();
}

void freeShader()
{
	glUseProgram(0);
	glDeleteProgram(Shader.ShaderProgram);
}

void resizeWindow(int width, int height)
{
	glViewport(0, 0, width, height);
}

struct Transform
{
	glm::mat4 model;
	glm::mat4 viewProjection;
	glm::mat3 normal;
	glm::vec3 viewPosition;
};

struct PointLight 
{
	glm::vec4 position;
	glm::vec4 ambient;
	glm::vec4 diffuse;
	glm::vec4 specular;
	glm::vec3 attenuation;
};

struct Material 
{
	glm::vec4 ambient;
	glm::vec4 diffuse;
	glm::vec4 specular;
	glm::vec4 emission;
	float shininess;
};

void render()
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	glm::vec3 viewPosition = glm::vec3(4, 3, 3);

	glm::mat4 Projection = glm::perspective(glm::radians(45.0f), 4.0f / 3.0f, 0.1f, 100.0f);
	glm::mat4 View = glm::lookAt(viewPosition, glm::vec3(0, 0, 0), glm::vec3(0, 1, 0));
	glm::mat4 Model = glm::mat4(1.0f);

	glm::mat4 rotate_x =
	{
		1.0f, 0.0f, 0.0f, 0.0f,
		0.0f, glm::cos(rotate_xx), -glm::sin(rotate_xx), 0.0f,
		0.0f, glm::sin(rotate_xx), glm::cos(rotate_xx), 0.0f,
		0.0f, 0.0f, 0.0f, 1.0f
	};

	glm::mat4 rotate_y =
	{
		glm::cos(rotate_yy), 0.0f, glm::sin(rotate_yy), 0.0f,
		0.0f, 1.0f, 0.0f, 0.0f,
		-glm::sin(rotate_yy), 0.0f, glm::cos(rotate_yy), 0.0f,
		0.0f, 0.0f, 0.0f, 1.0f
	};

	glm::mat4 rotate_z =
	{
		glm::cos(rotate_zz),  -glm::sin(rotate_zz), 0.0f, 0.0f,
		glm::sin(rotate_zz), glm::cos(rotate_zz), 0.0f, 0.0f,
		0.0f, 0.0f, 1.0f, 0.0f,
		0.0f, 0.0f, 0.0f, 1.0f
	};

	glm::mat4 MVP = Projection * View * Model * rotate_x * rotate_y * rotate_z;

	Transform transform;

	transform.model = Model * rotate_x * rotate_y * rotate_z;
	transform.viewProjection = Projection * View;
	transform.normal = glm::transpose(glm::inverse(transform.model));
	transform.viewPosition = viewPosition;//View point;

	PointLight light;
	light.position = glm::vec4(0, 3, 0, 1);
	light.ambient = glm::vec4(1, 1, 1, 1);
	if (mode == 5)
		light.diffuse = glm::vec4(1, 0.5, 0, 1);
	else
		light.diffuse = glm::vec4(1, 1, 0.8, 1);
	light.specular = glm::vec4(1, 1, 1, 1);
	light.attenuation = glm::vec3(0.5, 0.5, 0.5);

	Material material;
	material.ambient = glm::vec4(1, 1, 1, 1);
	if (mode == 4)
		material.diffuse = glm::vec4(0.8, 0, 0, 1);
	else
		material.diffuse = glm::vec4(2, 2, 2, 1);
	material.emission = glm::vec4(0, 0, 0, 1);
	material.specular = glm::vec4(10, 10, 10, 1);
	material.shininess = 16;

	glUseProgram(Shader.ShaderProgram);

	GLuint TransformModelID = glGetUniformLocation(Shader.ShaderProgram, "transform.model");
	GLuint TransformProjectionID = glGetUniformLocation(Shader.ShaderProgram, "transform.viewProjection");
	GLuint TransformNormalID = glGetUniformLocation(Shader.ShaderProgram, "transform.normal");
	GLuint TransformPositionID = glGetUniformLocation(Shader.ShaderProgram, "transform.viewPosition");
	glUniformMatrix4fv(TransformModelID, 1, GL_FALSE, &transform.model[0][0]);
	glUniformMatrix4fv(TransformProjectionID, 1, GL_FALSE, &transform.viewProjection[0][0]);
	glUniformMatrix3fv(TransformNormalID, 1, GL_FALSE, &transform.normal[0][0]);
	glUniform3fv(TransformPositionID, 1, &transform.viewPosition[0]);

	GLuint LightPositionID = glGetUniformLocation(Shader.ShaderProgram, "light.position");
	GLuint LightAmbientID = glGetUniformLocation(Shader.ShaderProgram, "light.ambient");
	GLuint LightDiffuseID = glGetUniformLocation(Shader.ShaderProgram, "light.diffuse");
	GLuint LightSpecularID = glGetUniformLocation(Shader.ShaderProgram, "light.specular");
	GLuint LightAttenuationID = glGetUniformLocation(Shader.ShaderProgram, "light.attenuation");
	glUniform4fv(LightPositionID, 1, &light.position[0]);
	glUniform4fv(LightAmbientID, 1, &light.ambient[0]);
	glUniform4fv(LightDiffuseID, 1, &light.diffuse[0]);
	glUniform4fv(LightSpecularID, 1, &light.specular[0]);
	glUniform3fv(LightAttenuationID, 1, &light.attenuation[0]);

	GLuint MaterialAmbientID = glGetUniformLocation(Shader.ShaderProgram, "material.ambient");
	GLuint MaterialDiffuseID = glGetUniformLocation(Shader.ShaderProgram, "material.diffuse");
	GLuint MaterialEmissionID = glGetUniformLocation(Shader.ShaderProgram, "material.emission");
	GLuint MaterialSpecularID = glGetUniformLocation(Shader.ShaderProgram, "material.specular");
	GLuint MaterialShininessID = glGetUniformLocation(Shader.ShaderProgram, "material.shininess");
	glUniform4fv(MaterialAmbientID, 1, &material.ambient[0]);
	glUniform4fv(MaterialDiffuseID, 1, &material.diffuse[0]);
	glUniform4fv(MaterialEmissionID, 1, &material.emission[0]);
	glUniform4fv(MaterialSpecularID, 1, &material.specular[0]);
	glUniform1f(MaterialShininessID, material.shininess);
	glActiveTexture(GL_TEXTURE1);
	glBindTexture(GL_TEXTURE_2D, texture_1);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glUniform1i(glGetUniformLocation(Shader.ShaderProgram, "material.texture"), 1);

	GLuint MatrixID = glGetUniformLocation(Shader.ShaderProgram, "MVP");
	glUniformMatrix4fv(MatrixID, 1, GL_FALSE, &MVP[0][0]);
	glEnableVertexAttribArray(0);
	glBindBuffer(GL_ARRAY_BUFFER, VBO_vertex);
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);
	glEnableVertexAttribArray(1);
	glBindBuffer(GL_ARRAY_BUFFER, VBO_texture);
	glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 0, (void*)0);
	if (mode == 3 || mode == 4 || mode == 5)
	{
		glEnableVertexAttribArray(2);
		glBindBuffer(GL_ARRAY_BUFFER, VBO_normal);
		glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);
	}
	else
	{
		glEnableVertexAttribArray(2);
		glBindBuffer(GL_ARRAY_BUFFER, VBO_color);
		glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);
	}
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, texture_1);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glUniform1i(glGetUniformLocation(Shader.ShaderProgram, "myTextureSampler"), 0);
	if (mode == 2)
	{
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, texture_2);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);

		glUniform1i(glGetUniformLocation(Shader.ShaderProgram, "myTextureSampler1"), 1);

		glUniform1f(glGetUniformLocation(Shader.ShaderProgram, "mix_f"), factor);
	}
	glDrawArrays(GL_TRIANGLES, 0, 1200 * 3);
	checkOpenGLerror();
	glutSwapBuffers();
}

void specialKeys(int key, int x, int y)
{
	switch (key)
	{
	case GLUT_KEY_UP: rotate_xx += 0.1; break;
	case GLUT_KEY_DOWN: rotate_xx -= 0.1; break;
	case GLUT_KEY_RIGHT: rotate_yy += 0.1; break;
	case GLUT_KEY_LEFT: rotate_yy -= 0.1; break;
	case GLUT_KEY_PAGE_UP: rotate_zz += 0.1; break;
	case GLUT_KEY_PAGE_DOWN: rotate_zz -= 0.1; break;
	case GLUT_KEY_F1: mode = 0; Init_Shader(); break;
	case GLUT_KEY_F2: mode = 1; Init_Shader(); break;
	case GLUT_KEY_F3: mode = 2; Init_Shader(); break;
	case GLUT_KEY_F4: mode = 3; Init_Shader(); break;
	case GLUT_KEY_F5: mode = 4; Init_Shader(); break;
	case GLUT_KEY_F6: mode = 5; Init_Shader(); break;
	case GLUT_KEY_F8:
	{
		if (factor > 0)
		{
			factor -= 0.1f;
		}

		break;
	}
	case GLUT_KEY_F9:
	{
		if (factor < 1)
		{
			factor += 0.1f;
		}

		break;
	}
	}
	glutPostRedisplay();
}

int main(int argc, char** argv)
{
	setlocale(LC_ALL, "rus");
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_DEPTH | GLUT_RGBA | GLUT_ALPHA | GLUT_DOUBLE);
	glutInitWindowSize(1000, 800);
	glutCreateWindow("Lab_13");
	glClearColor(0, 0, 0, 0);
	glEnable(GL_DEPTH_TEST);
	glDepthFunc(GL_LESS);

	GLenum glew_status = glewInit();

	mode = 3;
	factor = 0.2f;
	tex_Path_1 = "4.jpg";
	tex_Path_2 = "5.jpg";

	//Mesh mesh = openOBJ("cube_TRI.obj");
	//Mesh mesh = openOBJ("shar.obj");
	Mesh mesh = openOBJ("monkey_smol.obj");

	Init_Shader();
	Load_Textures();
	initVBO(mesh);
	glutReshapeFunc(resizeWindow);
	glutDisplayFunc(render);
	glutSpecialFunc(specialKeys);
	glutMainLoop();
	freeShader();
}