#include <SOIL.h>
#include <gl/glew.h> 
#include <gl/GL.h>
#include <gl/GLU.h>
#include <gl/freeglut.h>
#include <glm/trigonometric.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <iostream>

class ShaderUtils
{
private:
	GLuint vertex_shader;
	GLuint fragment_shader;

	GLuint compileSource(const char* source, GLuint shader_type)
	{
		GLuint shader = glCreateShader(shader_type);
		glShaderSource(shader, 1, &source, NULL);
		glCompileShader(shader);
		return shader;
	}

	void linkProgram()
	{
		ShaderProgram = glCreateProgram();
		glAttachShader(ShaderProgram, vertex_shader);
		glAttachShader(ShaderProgram, fragment_shader);
		glLinkProgram(ShaderProgram);
	}

public:
	GLuint ShaderProgram;

	ShaderUtils() :ShaderProgram(0) {}
	~ShaderUtils()
	{
		glUseProgram(0);
		glDeleteShader(vertex_shader);
		glDeleteShader(fragment_shader);
		glDeleteProgram(ShaderProgram);
	}

	void load(const char* vertext_src, const char* fragment_src)
	{
		vertex_shader = compileSource(vertext_src, GL_VERTEX_SHADER);
		fragment_shader = compileSource(fragment_src, GL_FRAGMENT_SHADER);
		linkProgram();
	}

	void load_vertex_shader(const char* vertext_src, const char* fragment_src)
	{
		vertex_shader = compileSource(vertext_src, GL_VERTEX_SHADER);
		fragment_shader = compileSource(fragment_src, GL_FRAGMENT_SHADER);
		linkProgram();
	}

	GLuint getIDProgram()
	{
		return ShaderProgram;
	}

	GLint getAttribLocation(const char* name)const
	{
		GLint t = glGetAttribLocation(ShaderProgram, name);
		if (t == -1)
		{
			std::cout << "could not bind attrib " << name << std::endl;
			return -1;
		}
		return t;
	}
	GLuint getUniformLocation(const char* name)const
	{
		GLint t = glGetUniformLocation(ShaderProgram, name);
		if (t == -1)
		{
			std::cout << "could not bind uniform " << name << std::endl;
			return -1;
		}
		return t;
	}
	void use() { glUseProgram(ShaderProgram); }
};
