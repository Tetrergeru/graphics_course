#include "ShaderUtility.h"
int mode = 0;
float factor = 0.0f;

std::string tex_Path_1 = "";
std::string tex_Path_2 = "";

ShaderUtils Shader;

GLuint VBO_vertex;
GLuint VBO_color;
GLuint VBO_texture;
GLuint texture_1;
GLuint texture_2;

const char* vsSource = "#version 330 core\n"
"layout(location = 0) in vec3 coord;\n"
"layout(location = 1) in vec2 vertexUV;\n"
"layout(location = 2) in vec3 vertexColor;\n"
"uniform mat4 MVP;\n"
"out vec2 UV;\n"
"out vec3 fragmentColor;\n"
"void main() {\n"
"	gl_Position = MVP * vec4(coord, 1.0);\n"
"	fragmentColor = vertexColor;\n"
"	UV = vertexUV;\n"
"}\n";

const char* fsSource_1 = "#version 330 core\n"
"in vec2 UV;\n"
"uniform sampler2D myTextureSampler;\n"
"in vec3 fragmentColor;\n"
"void main() {\n"
"	gl_FragColor = texture(myTextureSampler, UV);\n"
"}\n";

const char* fsSource_2 = "#version 330 core\n"
"in vec2 UV;\n"
"uniform sampler2D myTextureSampler;\n"
"in vec3 fragmentColor;\n"
"void main() {\n"
"	gl_FragColor = texture(myTextureSampler, UV) * vec4(fragmentColor, 1.0);\n"
"}\n";

const char* fsSource_3 = "#version 330 core\n"
"in vec2 UV;\n"
"uniform sampler2D myTextureSampler;\n"
"uniform sampler2D myTextureSampler1;\n"
"in vec3 fragmentColor;\n"
"uniform float mix_f;\n"
"void main() {\n"
"	gl_FragColor = mix(texture(myTextureSampler, UV), texture(myTextureSampler1, UV), mix_f);\n"
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
	if (mode == 0)
	{
		Shader.load(vsSource, fsSource_1);
	}
	else if (mode == 1)
	{
		Shader.load(vsSource, fsSource_2);
	}
	else
	{
		Shader.load(vsSource, fsSource_3);
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

void initVBO()
{
	static const GLfloat vertices[] =
	{
		-1, -1, -1,
		1, -1, -1,
		1, 1, -1,

		-1, -1, -1,
		1, 1, -1,
		-1, 1, -1,

		1, -1, -1,
		1, -1, 1,
		1, 1, 1,

		1, -1, -1,
		1, 1, 1,
		1, 1, -1,

		1, -1, 1,
		-1, -1, 1,
		-1, 1, 1,

		1, -1, 1,
		-1, 1, 1,
		1, 1, 1,

		-1, -1, 1,
		-1, -1, -1,
		-1, 1, -1,

		-1, -1, 1,
		-1, 1, -1,
		-1, 1, 1,

		-1, -1, -1,
		-1, -1, 1,
		1, -1, 1,

		-1, -1, -1,
		1, -1, 1,
		1, -1, -1,

		-1, 1, -1,
		-1, 1, 1,
		1, 1, 1,

		-1, 1, -1,
		1, 1, 1,
		1, 1, -1
	};

	GLfloat colors[] =
	{
		1.0f , 0.5f , 1.0f ,
		1.0f , 0.5f , 0.5f ,
		0.5f , 0.5f , 1.0f ,

		1.0f , 0.5f , 1.0f ,
		0.5f , 0.5f , 1.0f ,
		0.0f , 1.0f , 0.0f , 

		1.0f , 0.5f , 0.5f ,
		0.0f , 1.0f , 1.0f ,
		1.0f , 0.0f , 1.0f ,

		1.0f , 0.5f , 0.5f ,
		1.0f , 0.0f , 1.0f ,
		0.5f , 0.5f , 1.0f ,

		

		0.0f , 1.0f , 1.0f ,
		1.0f , 0.5f , 0.0f , 
		1.0f , 1.0f , 0.0f ,

		0.0f , 1.0f , 1.0f , 
		1.0f , 1.0f , 0.0f ,
		1.0f , 0.0f , 1.0f ,


		1.0f , 0.5f , 0.0f ,
		1.0f , 0.5f , 1.0f ,
		0.0f , 1.0f , 0.0f ,

		1.0f , 0.5f , 0.0f ,
		0.0f , 1.0f , 0.0f ,
		1.0f , 1.0f , 0.0f , 


		1.0f , 0.5f , 1.0f ,
		1.0f , 0.5f , 0.0f ,
		0.0f , 1.0f , 1.0f ,

		1.0f , 0.5f , 1.0f , 
		0.0f , 1.0f , 1.0f ,
		1.0f , 0.5f , 0.5f ,


		0.0f , 1.0f , 0.0f ,
		1.0f , 1.0f , 0.0f ,
		1.0f , 0.0f , 1.0f ,

		0.0f , 1.0f , 0.0f ,
		1.0f , 0.0f , 1.0f ,
		0.5f , 0.5f , 1.0f , 
	};

	GLfloat uvs[] =
	{
		0, 0,
		1, 0,
		1, 1,

		0, 0,
		1, 1,
		0, 1,

		0, 0,
		1, 0,
		1, 1,

		0, 0,
		1, 1,
		0, 1,

		0, 0,
		1, 0,
		1, 1,

		0, 0,
		1, 1,
		0, 1,

		0, 0,
		1, 0,
		1, 1,

		0, 0,
		1, 1,
		0, 1,

		0, 0,
		1, 0,
		1, 1,

		0, 0,
		1, 1,
		0, 1,

		0, 0,
		1, 0,
		1, 1,

		0, 0,
		1, 1,
		0, 1

	};

	glGenBuffers(1, &VBO_vertex);
	glBindBuffer(GL_ARRAY_BUFFER, VBO_vertex);
	glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

	glGenBuffers(1, &VBO_color);
	glBindBuffer(GL_ARRAY_BUFFER, VBO_color);
	glBufferData(GL_ARRAY_BUFFER, sizeof(colors), colors, GL_STATIC_DRAW);

	glGenBuffers(1, &VBO_texture);
	glBindBuffer(GL_ARRAY_BUFFER, VBO_texture);
	glBufferData(GL_ARRAY_BUFFER, sizeof(uvs), uvs, GL_STATIC_DRAW);

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

void render()
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	glm::mat4 Projection = glm::perspective(glm::radians(45.0f), 4.0f / 3.0f, 0.1f, 100.0f);
	glm::mat4 View = glm::lookAt(glm::vec3(4, 3, 3), glm::vec3(0, 0, 0), glm::vec3(0, 1, 0));
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

	glUseProgram(Shader.ShaderProgram);

	GLuint MatrixID = glGetUniformLocation(Shader.ShaderProgram, "MVP");
	glUniformMatrix4fv(MatrixID, 1, GL_FALSE, &MVP[0][0]);

	glEnableVertexAttribArray(0);
	glBindBuffer(GL_ARRAY_BUFFER, VBO_vertex);
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);

	glEnableVertexAttribArray(1);
	glBindBuffer(GL_ARRAY_BUFFER, VBO_texture);
	glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 0, (void*)0);

	glEnableVertexAttribArray(2);
	glBindBuffer(GL_ARRAY_BUFFER, VBO_color);
	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);

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

	glDrawArrays(GL_TRIANGLES, 0, 12 * 3);

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
	case GLUT_KEY_F10:
	{
		if (factor > 0)
		{
			factor -= 0.1f;
		}

		break;
	}
	case GLUT_KEY_F11:
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
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_DEPTH | GLUT_RGBA | GLUT_ALPHA | GLUT_DOUBLE);
	glutInitWindowSize(800, 600);
	glutCreateWindow("OpenGL");
	glEnable(GL_DEPTH_TEST);
	glDepthFunc(GL_LESS);

	GLenum glew_status = glewInit();

	mode = 0;
	factor = 0.4f; 
	tex_Path_1 = "4.jpg";
	tex_Path_2 = "3.jpg";

	Init_Shader();
	Load_Textures();
	initVBO();
	glutReshapeFunc(resizeWindow);
	glutDisplayFunc(render);
	glutSpecialFunc(specialKeys);
	glutMainLoop();
	freeShader();
}